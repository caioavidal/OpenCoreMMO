using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NeoServer.Data;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Providers.InMemoryDB.Extensions;
using NeoServer.Data.Providers.MySQL.Extensions;
using NeoServer.Data.Providers.SQLite.Extensions;
using NeoServer.Data.Repositories;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creature;
using NeoServer.Game.Creatures;
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
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Network.Enums;
using NeoServer.Server.Contracts.Tasks;
using NeoServer.Server.Events;
using NeoServer.Server.Handlers;
using NeoServer.Server.Handlers.Authentication;
using NeoServer.Server.Instances;
using NeoServer.Server.Jobs.Creatures;
using NeoServer.Server.Jobs.Items;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Tasks;
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

            builder.RegisterType<AccountRepositoryNeo>().As<IAccountRepositoryNeo>().SingleInstance();
            builder.RegisterType<PlayerDepotItemRepositoryNeo>().As<IPlayerDepotItemRepositoryNeo>().SingleInstance();

            builder.RegisterType<LoginProtocol>().SingleInstance();
            builder.RegisterType<LoginListener>().SingleInstance();
            builder.RegisterType<GameProtocol>().SingleInstance();
            builder.RegisterType<GameListener>().SingleInstance();

            builder.RegisterType<Game>().SingleInstance();
            builder.RegisterType<GameCreatureManager>().SingleInstance();
            builder.RegisterType<DecayableItemManager>().SingleInstance();

            builder.RegisterType<MonsterDataManager>().As<IMonsterDataManager>().SingleInstance();
            builder.RegisterType<SpawnManager>().SingleInstance();

            builder.RegisterType<NeoServer.Game.World.Map.PathFinder>().As<IPathFinder>().SingleInstance();

            builder.Register((c, p) =>
            {
                return new CreaturePathAccess(c.Resolve<IPathFinder>().Find, c.Resolve<IMap>().CanGoToDirection);
            }).SingleInstance();

            builder.RegisterPacketHandlers();

            builder.RegisterType<Scheduler>().As<IScheduler>().SingleInstance();
            //commands
            builder.RegisterType<Dispatcher>().As<IDispatcher>().SingleInstance();

            builder.RegisterServerEvents();
            builder.RegisterGameEvents();
            builder.RegisterEventSubscribers();

            builder.RegisterIncomingPacketFactory();
            builder.RegisterPlayerFactory();

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
            builder.RegisterCustomLoaders();
            builder.RegisterType<SpellLoader>().SingleInstance();


            //factories
            builder.RegisterType<ItemFactory>().As<IItemFactory>().OnActivated(e => e.Instance.ItemEventSubscribers = e.Context.Resolve<IEnumerable<IItemEventSubscriber>>()).SingleInstance();
            builder.RegisterType<LiquidPoolFactory>().As<ILiquidPoolFactory>().SingleInstance();

            builder.RegisterType<CreatureFactory>().As<ICreatureFactory>().SingleInstance();
            builder.RegisterType<MonsterFactory>().As<IMonsterFactory>().SingleInstance();

            //creature
            builder.RegisterType<CreatureGameInstance>().As<ICreatureGameInstance>().SingleInstance();

            builder.RegisterType<EventSubscriber>().SingleInstance();
            builder.RegisterType<GameCreatureJob>().SingleInstance();
            builder.RegisterType<GameItemJob>().SingleInstance();

            //Database
            builder.RegisterContext<NeoContext>();

            return builder.Build();
        }

        public static Logger RegisterLogger()
        {
            var logger = new LoggerConfiguration()
              .WriteTo.Console(theme: AnsiConsoleTheme.Code)
              .CreateLogger();
            Builder.RegisterInstance(logger).SingleInstance();
            return logger;
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
        private static void RegisterGameEvents(this ContainerBuilder builder)
        {
            var interfaceType = typeof(IGameEventHandler);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => interfaceType.IsAssignableFrom(x));

            foreach (var type in types)
            {
                if (type == interfaceType) continue;
                builder.RegisterType(type).SingleInstance();
                ;
            }
        }
        private static void RegisterEventSubscribers(this ContainerBuilder builder)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyTypes(types).As<ICreatureEventSubscriber>().SingleInstance();
            builder.RegisterAssemblyTypes(types).As<IItemEventSubscriber>().SingleInstance();
        }
        private static void RegisterPlayerLoaders(this ContainerBuilder builder)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyTypes(types).As<IPlayerLoader>().SingleInstance();
        }
        private static void RegisterCustomLoaders(this ContainerBuilder builder)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyTypes(types).As<ICustomLoader>().SingleInstance();
        }

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
                    return (IPacketHandler)instance;
                }

                return new NotImplementedPacketHandler(packet, c.Resolve<Logger>());
            });
        }
        private static IConfigurationRoot configuration;
        public static ServerConfiguration LoadConfigurations()
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
            ServerConfiguration serverConfiguration = new(0, null, null, null, "");
            GameConfiguration gameConfiguration = new();

            configuration.GetSection("server").Bind(serverConfiguration);
            configuration.GetSection("game").Bind(gameConfiguration);

            Builder.RegisterInstance(serverConfiguration).SingleInstance();
            Builder.RegisterInstance(gameConfiguration).SingleInstance();

            return serverConfiguration;
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
