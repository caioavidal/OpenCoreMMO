using Autofac;
using NeoServer.Data.RavenDB;
using NeoServer.Networking.Listeners;
using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Protocols;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Handlers;
using NeoServer.Server.Handlers.Authentication;
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

            BuildHandlerFactory(builder);
            BuildIncomingPacketFactory(builder);


            return builder.Build();


        }

        private static void BuildHandlerFactory(ContainerBuilder builder)
        {
            builder.Register<IEventHandler>((c, p) =>
            {
                var type = p.TypedAs<GameIncomingPacketType>();

                switch (type)
                {
                    case GameIncomingPacketType.AddVip:
                        return new AccountLoginEventHandler(c.Resolve<AccountRepository>());
                    default:
                        throw new ArgumentException("Invalid game incoming type");
                }
            });
        }

        private static void BuildIncomingPacketFactory(ContainerBuilder builder)
        {
            builder.Register<IncomingPacket>((c, p) =>
            {
                var inMessage = p.TypedAs<NetworkMessage>();

                switch (inMessage.IncomingPacketType)
                {
                    case GameIncomingPacketType.AddVip:
                        return new AccountLoginPacket(inMessage);
                    default:
                        throw new ArgumentException("Invalid game incoming type");
                }
            });
        }
    }
}
