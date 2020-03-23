using Autofac;
using NeoServer.Data.RavenDB;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creature;
using NeoServer.Networking;
using NeoServer.Networking.Listeners;
using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Protocols;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Handlers;
using NeoServer.Server.Handlers.Authentication;
using NeoServer.Server.Model.Items;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Standalone.Factories;
using NeoServer.Server.World;
using NeoServer.Server.World.Map;

namespace NeoServer.Server.Standalone.IoC
{
    public class Container
    {
        public static IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();

            //server
            builder.RegisterType<ServerState>().SingleInstance();

            builder.RegisterType<Database>().SingleInstance();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();


            builder.RegisterType<LoginProtocol>();
            builder.RegisterType<LoginListener>();
            builder.RegisterType<GameProtocol>();
            builder.RegisterType<GameListener>();

            builder.RegisterType<Game>().SingleInstance();

            builder.RegisterType<AccountLoginEventHandler>().SingleInstance();
            builder.RegisterType<PlayerLogInEventHandler>().SingleInstance();
            builder.RegisterType<PlayerChangesModeEventHandler>().SingleInstance();
            builder.RegisterType<PlayerLogOutEventHandler>().SingleInstance();


            builder.RegisterType<AccountLoginPacket>();
            //builder.RegisterType<PlayerLoginPacket>();

            RegisterIncomingPacketFactory(builder);

            RegisterItemFactory(builder);

            RegisterPlayerFactory(builder);

            //world
            builder.RegisterType<World.World>().SingleInstance();
            builder.RegisterType<WorldLoader>().As<IWorldLoader>();
            builder.RegisterType<OTBMWorldLoader>();
            builder.RegisterType<World.Map.Map>().SingleInstance();
            builder.RegisterType<CreatureDescription>();


            //factories
            builder.RegisterType<PlayerFactory>().SingleInstance();

            //creature
            builder.RegisterType<CreatureGameInstance>().As<ICreatureGameInstance>().SingleInstance();

            return builder.Build();


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
                var conn = p.TypedAs<Connection>();

                var packet = conn.InMessage.GetIncomingPacketType(conn.IsAuthenticated);

                var handlerType = IncomingPacketHandlerData.Data[packet];

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
