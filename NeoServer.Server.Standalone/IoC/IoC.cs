using Autofac;
using NeoServer.Data.RavenDB;
using NeoServer.Networking.Listeners;
using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Protocols;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Handlers.Authentication;
using NeoServer.Server.Model.Items;
using NeoServer.Server.World;

namespace NeoServer.Server.Standalone.IoC
{
    public class Container
    {
        public static IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Database>().SingleInstance();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();


            builder.RegisterType<LoginProtocol>();
            builder.RegisterType<LoginListener>();
            builder.RegisterType<GameProtocol>();
            builder.RegisterType<GameListener>();

            //builder.RegisterType<NetworkQueue>().As<INetworkQueue>();
            //builder.RegisterType<OutputStreamMessage>().As<IOutputStreamMessage>();

            builder.RegisterType<AccountLoginEventHandler>().SingleInstance();

            builder.RegisterType<AccountLoginPacket>();
            builder.RegisterType<PlayerLoginPacket>();

            RegisterIncomingPacketFactory(builder);

            RegisterItemFactory(builder);

            //world
            builder.RegisterType<World.World>().SingleInstance();
            builder.RegisterType<WorldLoader>().As<IWorldLoader>();
            builder.RegisterType<OTBMWorldLoader>();
            builder.RegisterType<World.Map.Map>();

            return builder.Build();


        }



        private static void RegisterIncomingPacketFactory(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var networkMessage = p.TypedAs<IReadOnlyNetworkMessage>();

                var packetType = IncomingDictionaryData.Data[networkMessage.IncomingPacketType];

                return (IncomingPacket) c.Resolve(packetType, new PositionalParameter(0, networkMessage));
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
