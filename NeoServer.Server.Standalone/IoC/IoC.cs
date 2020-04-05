using System;
using System.Reflection;
using Autofac;
using NeoServer.Data.RavenDB;
using NeoServer.Game.Commands;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creature;
using NeoServer.Loaders.Items;
using NeoServer.Networking.Listeners;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Protocols;
using NeoServer.OTBM;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Network.Enums;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Events;
using NeoServer.Server.Handlers;
using NeoServer.Server.Handlers.Authentication;
using NeoServer.Server.Handlers.Players;
using NeoServer.Server.Model.Items;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Schedulers;
using NeoServer.Server.Schedulers.Contracts;
using NeoServer.Server.Standalone.Factories;
using NeoServer.Server.World;
using NeoServer.Server.World.Map;
using Serilog;
using Serilog.Core;

namespace NeoServer.Server.Standalone.IoC
{
    public class Container
    {
        public static Autofac.IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();

            //server
            builder.RegisterType<ServerState>().SingleInstance();

            builder.RegisterInstance<Logger>(new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger()).SingleInstance();

            builder.RegisterType<Database>().SingleInstance();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();


            builder.RegisterType<LoginProtocol>();
            builder.RegisterType<LoginListener>();
            builder.RegisterType<GameProtocol>();
            builder.RegisterType<GameListener>();

            builder.RegisterType<Game>().SingleInstance();

            RegisterPacketHandlers(builder);

            builder.RegisterType<Scheduler>().As<IScheduler>().SingleInstance();
            //commands
            builder.RegisterType<Dispatcher>().As<IDispatcher>().SingleInstance();

            RegisterEvents(builder);
            //RegisterCommands(builder);

            RegisterIncomingPacketFactory(builder);

            RegisterItemFactory(builder);

            RegisterPlayerFactory(builder);

            //world
            builder.RegisterType<World.World>().SingleInstance();
            //builder.RegisterType<Server.World.WorldLoader>().As<IWorldLoader>();

            builder.RegisterType<ItemTypeLoader>();
            builder.RegisterType<Loaders.World.WorldLoader>();

            //builder.RegisterType<OTBMWorldLoader>();
            builder.RegisterType<World.Map.Map>().As<IMap>().SingleInstance();
            builder.RegisterType<CreatureDescription>();

            //factories
            builder.RegisterType<PlayerFactory>().SingleInstance();

            //creature
            builder.RegisterType<CreatureGameInstance>().As<ICreatureGameInstance>().SingleInstance();

            return builder.Build();
        }

        private static void RegisterPacketHandlers(ContainerBuilder builder)
        {
            var assemblies = Assembly.GetAssembly(typeof(PacketHandler));
            builder.RegisterAssemblyTypes(assemblies);
        }

        private static void RegisterCommands(ContainerBuilder builder)
        {
            var assembly = Assembly.GetAssembly(typeof(PlayerLogInCommandHandler));

            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ICommandHandler<>));
        }
        private static void RegisterEvents(ContainerBuilder builder)
        {
            var assembly = Assembly.GetAssembly(typeof(PlayerAddedOnMapEventHandler));
            var assemblyCommands = Assembly.GetAssembly(typeof(PlayerLogInCommandHandler));

            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IEventHandler<>));
            builder.RegisterAssemblyTypes(assemblyCommands).AsClosedTypesOf(typeof(IEventHandler<>));
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

                if (!IncomingPacketHandlerData.Data.TryGetValue(packet, out handlerType))
                {
                    Console.WriteLine($"Incoming Packet not handled: {packet}");
                }
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
