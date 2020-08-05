using Autofac;
using NeoServer.Data.RavenDB;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creature;
using NeoServer.Game.Creatures;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Items;
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
using NeoServer.Server.Jobs.Creatures;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Standalone.Factories;
using NeoServer.Server.Tasks;
using NeoServer.Server.Tasks.Contracts;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NeoServer.Server.Standalone.IoC
{
    public class Container
    {
        public static Autofac.IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();

            //server
            builder.RegisterType<GameState>().SingleInstance();

            builder.RegisterInstance(new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger()).SingleInstance();

            builder.RegisterType<Database>().SingleInstance();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>().SingleInstance();


            builder.RegisterType<LoginProtocol>().SingleInstance();
            builder.RegisterType<LoginListener>().SingleInstance();
            builder.RegisterType<GameProtocol>().SingleInstance();
            builder.RegisterType<GameListener>().SingleInstance();

            builder.RegisterType<Game>().SingleInstance();
            builder.RegisterType<GameCreatureManager>().SingleInstance();
            builder.RegisterType<MonsterDataManager>().As<IMonsterDataManager>().SingleInstance();
            builder.RegisterType<SpawnManager>().SingleInstance();

            RegisterPacketHandlers(builder);

            builder.RegisterType<Scheduler>().As<IScheduler>().SingleInstance();
            //commands
            builder.RegisterType<Dispatcher>().As<IDispatcher>().SingleInstance();

            RegisterEvents(builder);


            RegisterIncomingPacketFactory(builder);

            RegisterItemFactory(builder);
            RegisterNewItemFactory(builder);

            RegisterPlayerFactory(builder);

            //world
            builder.RegisterType<World>().SingleInstance();
            //builder.RegisterType<Server.World.WorldLoader>().As<IWorldLoader>();

            builder.RegisterType<ItemTypeLoader>().SingleInstance();
            builder.RegisterType<Loaders.World.WorldLoader>().SingleInstance();
            builder.RegisterType<SpawnLoader>().SingleInstance();
            builder.RegisterType<MonsterLoader>().SingleInstance();

            //builder.RegisterType<OTBMWorldLoader>();
            builder.RegisterType<Map>().As<IMap>().SingleInstance();

            //factories
            builder.RegisterType<PlayerFactory>().SingleInstance();
            builder.RegisterType<MonsterFactory>().As<IMonsterFactory>().SingleInstance();

            //creature
            builder.RegisterType<CreatureGameInstance>().As<ICreatureGameInstance>().SingleInstance();

            builder.RegisterType<EventSubscriber>().SingleInstance();
            builder.RegisterType<GameCreatureJob>().SingleInstance();

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
                var player = p.TypedAs<PlayerModel>();

                return c.Resolve<PlayerFactory>().Create(player);
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
                Console.WriteLine($"Incoming Packet: {packet}");

                if (c.TryResolve(handlerType, out object instance))
                {
                    return (IPacketHandler)instance;
                }
                return null;
            });
        }

        private static void RegisterItemFactory(ContainerBuilder builder)
        {
            //builder.Register((c, p) =>
            //{
            //    var typeId = p.TypedAs<ushort>();


            //    return ItemFactory.Create(typeId);
            //});
        }

        private static void RegisterNewItemFactory(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var typeId = p.TypedAs<ushort>();
                var location = p.TypedAs<Location>();
                var attributes = p.TypedAs<IDictionary<ItemAttribute, IConvertible>>();


                return ItemFactory.Create(typeId, location, attributes);
            });
        }

    }
}
