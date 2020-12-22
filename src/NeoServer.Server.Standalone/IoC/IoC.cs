using Autofac;
using NeoServer.Data.RavenDB;
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
using NeoServer.Loaders.Items;
using NeoServer.Loaders.Monsters;
using NeoServer.Loaders.Spawns;
using NeoServer.Networking.Listeners;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Protocols;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Network.Enums;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Events;
using NeoServer.Server.Handlers;
using NeoServer.Server.Instances;
using NeoServer.Server.Jobs.Creatures;
using NeoServer.Server.Jobs.Items;
using NeoServer.Server.Tasks;
using NeoServer.Server.Contracts.Tasks;
using Serilog;
using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.IO;
using NeoServer.Game.Common;
using NeoServer.Game.World.Map.Operations;
using NeoServer.Data.Repositories;
using NeoServer.Loaders.Vocations;

namespace NeoServer.Server.Standalone.IoC
{
    public class Container
    {
        public static Autofac.IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();

            //server

            builder.RegisterInstance(new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger()).SingleInstance();

            builder.RegisterType<Database>().SingleInstance();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>().SingleInstance();
            builder.RegisterType<PlayerDepotRepository>().As<IPlayerDepotRepository>().SingleInstance();

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

            RegisterPacketHandlers(builder);

            builder.RegisterType<Scheduler>().As<IScheduler>().SingleInstance();
            //commands
            builder.RegisterType<Dispatcher>().As<IDispatcher>().SingleInstance();

            RegisterEvents(builder);

            RegisterIncomingPacketFactory(builder);
            RegisterPlayerFactory(builder);
            LoadConfigurations(builder);

            //world
            builder.RegisterType<Map>().As<IMap>().SingleInstance();
            builder.RegisterType<World>().SingleInstance();

            //loaders
            builder.RegisterType<ItemTypeLoader>().SingleInstance();
            builder.RegisterType<Loaders.World.WorldLoader>().SingleInstance();
            builder.RegisterType<SpawnLoader>().SingleInstance();
            builder.RegisterType<MonsterLoader>().SingleInstance();
            builder.RegisterType<VocationLoader>().SingleInstance();

            //factories
            builder.RegisterType<ItemFactory>().As<IItemFactory>().SingleInstance();
            builder.RegisterType<LiquidPoolFactory>().As<ILiquidPoolFactory>().SingleInstance();

            builder.RegisterType<PlayerFactory>().As<IPlayerFactory>().SingleInstance();
            builder.RegisterType<CreatureFactory>().As<ICreatureFactory>().SingleInstance();
            builder.RegisterType<MonsterFactory>().As<IMonsterFactory>().SingleInstance();

            //creature
            builder.RegisterType<CreatureGameInstance>().As<ICreatureGameInstance>().SingleInstance();

            builder.RegisterType<EventSubscriber>().SingleInstance();
            builder.RegisterType<GameCreatureJob>().SingleInstance();
            builder.RegisterType<GameItemJob>().SingleInstance();

            return builder.Build();
        }

        private static void RegisterPacketHandlers(ContainerBuilder builder)
        {
            var assemblies = Assembly.GetAssembly(typeof(PacketHandler));
            builder.RegisterAssemblyTypes(assemblies).SingleInstance();
        }

        private static void RegisterEvents(ContainerBuilder builder)
        {
            var assembly = Assembly.GetAssembly(typeof(PlayerAddedOnMapEventHandler));

            builder.RegisterAssemblyTypes(assembly);

        }

        private static void RegisterPlayerFactory(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var player = p.TypedAs<IPlayerModel>();

                return c.Resolve<ICreatureFactory>().CreatePlayer(player);
            });
        }

        private static void RegisterIncomingPacketFactory(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var conn = p.TypedAs<IConnection>();

                var packet = GameIncomingPacketType.PlayerLogOut;

                if (!conn.Disconnected)
                {
                    packet = conn.InMessage.GetIncomingPacketType(conn.IsAuthenticated);
                }

                Type handlerType = null;

                if (!InputHandlerMap.Data.TryGetValue(packet, out handlerType))
                {
                    Console.WriteLine($"Incoming Packet not handled: {packet}");
                    return null;
                }
                // Console.WriteLine($"Incoming Packet: {packet}");

                if (c.TryResolve(handlerType, out object instance))
                {
                    return (IPacketHandler)instance;
                }
                return null;
            });
        }

        static void LoadConfigurations(ContainerBuilder containerBuilder)
        {
            var builder = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            ServerConfiguration serverConfiguration = new(0, null, null, null);
            GameConfiguration gameConfiguration = new();

            configuration.GetSection("server").Bind(serverConfiguration);
            configuration.GetSection("game").Bind(gameConfiguration);

            containerBuilder.RegisterInstance(serverConfiguration).SingleInstance();
            containerBuilder.RegisterInstance(gameConfiguration).SingleInstance();

        }
    }
}
