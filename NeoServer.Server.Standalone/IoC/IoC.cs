using Autofac;
using NeoServer.Data.RavenDB;
using NeoServer.Networking.Listeners;
using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Protocols;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Handlers.Authentication;

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

            RegisterIncomingPacketFactory(builder);

            return builder.Build();


        }



        private static void RegisterIncomingPacketFactory(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var networkMessage = p.TypedAs<NetworkMessage>();

                var packetType = IncomingDictionaryData.Data[GameIncomingPacketType.PlayerLoginRequest];

                return (IncomingPacket) c.Resolve(packetType, new PositionalParameter(0, networkMessage));
            });
        }
    }
}
