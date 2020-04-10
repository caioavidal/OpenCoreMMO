using System;
using System.Reflection;
using Autofac;
using NeoServer.Data.RavenDB;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creature;
using NeoServer.Loaders.Items;
using NeoServer.Networking.Listeners;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Protocols;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Network.Enums;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Handlers;
using NeoServer.Game.Items;
using NeoServer.Server.Model.Players;

using NeoServer.Server.Standalone.Factories;
using NeoServer.Game.World.Map;
using Serilog;
using Serilog.Core;
using NeoServer.Game.World;
using NeoServer.Server.Tasks;
using NeoServer.Server.Tasks.Contracts;
using NeoServer.Server.Events;
using NeoServer.Server.Jobs.Creatures;

namespace NeoServer.Server.Standalone.IoC
{
    public class Container
    {
        public static Autofac.IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();

            //server
            builder.RegisterType<GameState>().SingleInstance();

            builder.RegisterInstance<Logger>(new LoggerConfiguration()
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

            RegisterPacketHandlers(builder);

            builder.RegisterType<Scheduler>().As<IScheduler>().SingleInstance();
            //commands
            builder.RegisterType<Dispatcher>().As<IDispatcher>().SingleInstance();

            RegisterEvents(builder);
            

            RegisterIncomingPacketFactory(builder);

            RegisterItemFactory(builder);

            RegisterPlayerFactory(builder);

            //world
            builder.RegisterType<World>().SingleInstance();
            //builder.RegisterType<Server.World.WorldLoader>().As<IWorldLoader>();

            builder.RegisterType<ItemTypeLoader>().SingleInstance(); ;
            builder.RegisterType<Loaders.World.WorldLoader>().SingleInstance(); ;

            //builder.RegisterType<OTBMWorldLoader>();
            builder.RegisterType<Map>().As<IMap>().SingleInstance();
            builder.RegisterType<CreatureDescription>();

            //factories
            builder.RegisterType<PlayerFactory>().SingleInstance();

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
                    
                }
                Console.WriteLine($"Incoming Packet: {packet}");
                return (IPacketHandler)c.Resolve(handlerType);
            });
        }

        private static void RegisterItemFactory(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var typeId = p.TypedAs<ushort>();


                return ItemFactory.Create(typeId);
            });
        }
    }
}
