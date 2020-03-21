using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Handlers;
using NeoServer.Server.Handlers.Authentication;
using NeoServer.Server.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Protocols
{
    public class LoginProtocol : OpenTibiaProtocol
    {
        public override bool KeepConnectionOpen => false;
        private Func<Connection, IPacketHandler> _handlerFactory;
        public LoginProtocol(Func<Connection, IPacketHandler> packetFactory)
        {
            _handlerFactory = packetFactory;
        }

        public override void ProcessMessage(object sender, ConnectionEventArgs args)
        {
            var handler = _handlerFactory(args.Connection);
            handler.HandlerMessage(args.Connection.InMessage, args.Connection);
        }
    }
}
