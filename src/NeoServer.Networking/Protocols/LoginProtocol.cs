using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Handlers;
using System;

namespace NeoServer.Networking.Protocols
{
    public class LoginProtocol : Protocol
    {
        public override bool KeepConnectionOpen => false;
        private Func<IConnection, IPacketHandler> _handlerFactory;
        public LoginProtocol(Func<IConnection, IPacketHandler> packetFactory)
        {
            _handlerFactory = packetFactory;
        }

        public override void ProcessMessage(object sender, IConnectionEventArgs args)
        {
            var handler = _handlerFactory(args.Connection);
            handler.HandlerMessage(args.Connection.InMessage, args.Connection);
        }

        public override string ToString() => "Login Protocol";

    }
}
