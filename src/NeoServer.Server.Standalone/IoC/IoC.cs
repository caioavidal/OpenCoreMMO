using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using NeoServer.Data;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Providers.InMemoryDB.Extensions;
using NeoServer.Data.Providers.MySQL.Extensions;
using NeoServer.Data.Providers.SQLite.Extensions;
using NeoServer.Data.Repositories;
using NeoServer.Game.Chats;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creature;
using NeoServer.Game.Creatures;
using NeoServer.Game.Creatures.Events;
using NeoServer.Game.Creatures.Services;
using NeoServer.Game.Items;
using NeoServer.Game.Items.Factories;
using NeoServer.Game.World;
using NeoServer.Game.World.Map;
using NeoServer.Game.World.Spawns;
using NeoServer.Loaders.Interfaces;
using NeoServer.Loaders.Items;
using NeoServer.Loaders.Monsters;
using NeoServer.Loaders.Spawns;
using NeoServer.Loaders.Spells;
using NeoServer.Loaders.Vocations;
using NeoServer.Networking.Listeners;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Protocols;
using NeoServer.Scripts.Lua;
using NeoServer.Server.Commands;
using NeoServer.Server.Commands.Movement;
using NeoServer.Server.Commands.Player.UseItem;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Network.Enums;
using NeoServer.Server.Contracts.Tasks;
using NeoServer.Server.Events;
using NeoServer.Server.Handlers;
using NeoServer.Server.Handlers.Authentication;
using NeoServer.Server.Instances;
using NeoServer.Server.Jobs.Creatures;
using NeoServer.Server.Jobs.Items;
using NeoServer.Server.Jobs.Persistance;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Tasks;
using NLua;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NeoServer.Server.Standalone.IoC
{
    public static class Container
    {
        private static ContainerBuilder builder;

        public static ContainerBuilder Builder
        {
            get
            {
                if (builder is null) builder = new ContainerBuilder();
                return builder;
            }
        }

        public static Autofac.IContainer CompositionRoot()
        {
            var builder = Builder;
            //server

            builder.RegisterType<AccountRepository>().As<IAccountRepository>().SingleInstance();
            builder.RegisterType<GuildRepository>().As<IGuildRepository>().SingleInstance();
            builder.RegisterType<PlayerDepotItemRepositoryNeo>().As<IPlayerDepotItemRepositoryNeo>().SingleInstance();

            builder.RegisterInstance(new Lua()).SingleInstance();
            builder.RegisterType<LuaGlobalRegister>().SingleInstance();

            builder.RegisterType<LoginProtocol>().SingleInstance();
            builder.RegisterType<LoginListener>().SingleInstance();
            builder.RegisterType<GameProtocol>().SingleInstance();
            builder.RegisterType<GameListener>().SingleInstance();

            builder.RegisterType<GameServer>().As<IGameServer>().SingleInstance();
            builder.RegisterType<GameCreatureManager>().As<IGameCreatureManager>().SingleInstance();
            builder.RegisterType<DecayableItemManager>().SingleInstance();

            builder.RegisterType<MonsterDataManager>().As<IMonsterDataManager>().SingleInstance();
            builder.RegisterType<SpawnManager>().SingleInstance();

            //tools
            builder.RegisterType<NeoServer.Game.World.Map.PathFinder>().As<IPathFinder>().SingleInstance();
            builder.RegisterType<WalkToMechanism>().As<IWalkToMechanism>().SingleInstance();

            builder.RegisterPacketHandlers();

            builder.RegisterType<OptimizedScheduler>().As<IScheduler>().SingleInstance();
            //commands
            builder.RegisterType<Dispatcher>().As<IDispatcher>().SingleInstance();

            builder.RegisterServerEvents();
            builder.RegisterGameEvents();
            builder.RegisterEventSubscribers();
            builder.RegisterCommands();
            builder.RegisterIncomingPacketFactory();
            builder.RegisterPlayerFactory();
            builder.RegisterCustomLoaders();

            //world
            builder.RegisterType<Map>().As<IMap>().SingleInstance();
            builder.RegisterType<World>().SingleInstance();

            //loaders
            builder.RegisterType<ItemTypeLoader>().SingleInstance();
            builder.RegisterType<Loaders.World.WorldLoader>().SingleInstance();
            builder.RegisterType<SpawnLoader>().SingleInstance();
            builder.RegisterType<MonsterLoader>().SingleInstance();
            builder.RegisterType<VocationLoader>().SingleInstance();
            builder.RegisterPlayerLoaders();
            builder.RegisterStartupLoaders();
            builder.RegisterType<SpellLoader>().SingleInstance();

            //factories
            builder.RegisterType<ItemFactory>().As<IItemFactory>().OnActivated(e => e.Instance.ItemEventSubscribers = e.Context.Resolve<IEnumerable<IItemEventSubscriber>>()).SingleInstance();
            builder.RegisterType<ChatChannelFactory>().OnActivated(e => e.Instance.ChannelEventSubscribers = e.Context.Resolve<IEnumerable<IChatChannelEventSubscriber>>()).SingleInstance();
            builder.RegisterType<LiquidPoolFactory>().As<ILiquidPoolFactory>().SingleInstance();

            builder.RegisterType<CreatureFactory>().As<ICreatureFactory>().SingleInstance();
            builder.RegisterType<MonsterFactory>().As<IMonsterFactory>().SingleInstance();
            builder.RegisterType<NpcFactory>().As<INpcFactory>().SingleInstance();

            //creature
            builder.RegisterType<CreatureGameInstance>().As<ICreatureGameInstance>().SingleInstance();

            //services
            builder.RegisterType<DealTransaction>().As<IDealTransaction>().SingleInstance();
            builder.RegisterType<CoinTransaction>().As<ICoinTransaction>().SingleInstance();
            builder.RegisterType<PartyInviteService>().As<IPartyInviteService>().SingleInstance();
            builder.RegisterType<SummonService>().As<ISummonService>().SingleInstance();

            builder.RegisterType<EventSubscriber>().SingleInstance();

            //todo: inherit these jobs from interface and register by implementation
            builder.RegisterType<GameCreatureJob>().SingleInstance();
            builder.RegisterType<GameItemJob>().SingleInstance();
            builder.RegisterType<GameChatChannelJob>().SingleInstance();
            builder.RegisterType<PlayerPersistenceJob>().SingleInstance();

            builder.RegisterType<HotKeyCache>().SingleInstance();
            builder.RegisterInstance(new MemoryCache(new MemoryCacheOptions())).As<IMemoryCache>();

            //Database
            builder.RegisterContext<NeoContext>();

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).As<IStartup>().SingleInstance();
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).As<IRunBeforeLoaders>().SingleInstance();

            return builder.Build();
        }

        public static (Logger, LoggerConfiguration) RegisterLogger()
        {
            var loggerConfig = new LoggerConfiguration()
              .ReadFrom.Configuration(configuration, sectionName: "Log")
              .WriteTo.Console(theme: AnsiConsoleTheme.Code);

            var logger = loggerConfig.CreateLogger();

            Builder.RegisterInstance(logger).SingleInstance();

            return (logger, loggerConfig);
        }

        private static void RegisterPacketHandlers(this ContainerBuilder builder)
        {
            var assemblies = Assembly.GetAssembly(typeof(PacketHandler));
            builder.RegisterAssemblyTypes(assemblies).SingleInstance();
        }

        private static void RegisterServerEvents(this ContainerBuilder builder)
        {
            var assembly = Assembly.GetAssembly(typeof(PlayerAddedOnMapEventHandler));
            builder.RegisterAssemblyTypes(assembly);
        }
        private static void RegisterGameEvents(this ContainerBuilder builder) => RegisterAssembliesByInterface(typeof(IGameEventHandler));

        private static void RegisterEventSubscribers(this ContainerBuilder builder)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyTypes(types).As<ICreatureEventSubscriber>().SingleInstance();
            builder.RegisterAssemblyTypes(types).As<IItemEventSubscriber>().SingleInstance();
            builder.RegisterAssemblyTypes(types).As<IChatChannelEventSubscriber>().SingleInstance();
        }

        private static void RegisterAssembliesByInterface(Type interfaceType)
        {

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => interfaceType.IsAssignableFrom(x));

            foreach (var type in types)
            {
                if (type == interfaceType) continue;
                builder.RegisterType(type).SingleInstance();

            }
        }
        private static void RegisterCommands(this ContainerBuilder builder)
        {
            var assembly = Assembly.GetAssembly(typeof(PlayerLogInCommand));
            builder.RegisterAssemblyTypes(assembly);
        }

        private static void RegisterPlayerLoaders(this ContainerBuilder builder)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyTypes(types).As<IPlayerLoader>().SingleInstance();
        }
        private static void RegisterStartupLoaders(this ContainerBuilder builder)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyTypes(types).As<IStartupLoader>().SingleInstance();
        }
        private static void RegisterCustomLoaders(this ContainerBuilder builder) => RegisterAssembliesByInterface(typeof(ICustomLoader));

        private static void RegisterPlayerFactory(this ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var player = p.TypedAs<Player>();

                return c.Resolve<ICreatureFactory>().CreatePlayer(player);
            });
        }

        private static void RegisterIncomingPacketFactory(this ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var conn = p.TypedAs<IConnection>();

                var packet = GameIncomingPacketType.PlayerLogOut;

                if (!conn.Disconnected)
                {
                    packet = conn.InMessage.GetIncomingPacketType(conn.IsAuthenticated);
                }

                if (!InputHandlerMap.Data.TryGetValue(packet, out Type handlerType))
                {
                    return new NotImplementedPacketHandler(packet, c.Resolve<Logger>());
                }

                if (c.TryResolve(handlerType, out object instance))
                {
                    c.Resolve<Logger>().Debug("{incoming}: {packet}", "Incoming Packet", packet);

                    return (IPacketHandler)instance;
                }

                return new NotImplementedPacketHandler(packet, c.Resolve<Logger>());
            });
        }
        private static IConfigurationRoot configuration;
        public static (ServerConfiguration, GameConfiguration, LogConfiguration) LoadConfigurations()
        {

            var environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                       .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                       .AddEnvironmentVariables();//.Build();

            //only add secrets in development
            if (environmentName.Equals("Local", StringComparison.InvariantCultureIgnoreCase))
            {
                builder.AddUserSecrets<Program>();
            }
            configuration = builder.Build();
            ServerConfiguration serverConfiguration = new(0, null, null, null, string.Empty, string.Empty, new(3600));
            GameConfiguration gameConfiguration = new();
            LogConfiguration logConfiguration = new(null);

            configuration.GetSection("server").Bind(serverConfiguration);
            configuration.GetSection("game").Bind(gameConfiguration);
            configuration.GetSection("log").Bind(logConfiguration);

            Builder.RegisterInstance(serverConfiguration).SingleInstance();
            Builder.RegisterInstance(gameConfiguration).SingleInstance();
            Builder.RegisterInstance(logConfiguration).SingleInstance();

            return (serverConfiguration, gameConfiguration, logConfiguration);
        }

        private static void RegisterContext<TContext>(this ContainerBuilder builder) where TContext : DbContext
        {

            DatabaseConfiguration2 config = new(null, DatabaseType.INMEMORY);
            configuration.GetSection("database").Bind(config);

            DbContextOptions<NeoContext> options = config.active switch
            {
                DatabaseType.INMEMORY => DbContextFactory.GetInstance().UseInMemory(config.connections[DatabaseType.INMEMORY]),
                DatabaseType.MONGODB => DbContextFactory.GetInstance().UseInMemory(config.connections[DatabaseType.MONGODB]),
                DatabaseType.MYSQL => DbContextFactory.GetInstance().UseMySql(config.connections[DatabaseType.MYSQL]),
                DatabaseType.MSSQL => DbContextFactory.GetInstance().UseInMemory(config.connections[DatabaseType.MSSQL]),
                DatabaseType.SQLITE => DbContextFactory.GetInstance().UseSQLite(config.connections[DatabaseType.SQLITE]),
                _ => throw new ArgumentException("Invalid active database!"),
            };

            builder.RegisterInstance(config).SingleInstance();

            builder.RegisterType<TContext>()
                   .WithParameter("options", options)
                   .InstancePerLifetimeScope();
        }

    }
}
