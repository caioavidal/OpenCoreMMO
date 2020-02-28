using Autofac;
using NeoServer.Data.RavenDB;
using NeoServer.Networking;
using NeoServer.Networking.Listeners;
using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Protocols;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Handlers;
using NeoServer.Server.Handlers.Authentication;
using NeoServer.Server.Model;
using NeoServer.Server.Security;
using System;

namespace NeoServer.Server.Standalone.IoC
{
    public class Container
    {
        public static IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<AccountRepository>().As<IAccountRepository>();

            builder.RegisterType<LoginProtocol>();
            builder.RegisterType<LoginListener>();

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
