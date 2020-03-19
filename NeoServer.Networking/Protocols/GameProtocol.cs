using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Protocols
{
    public class GameProtocol : OpenTibiaProtocol
    {
        private Func<IReadOnlyNetworkMessage, IPacketHandler> _handlerFactory;
        public GameProtocol(Func<IReadOnlyNetworkMessage, IPacketHandler> handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        public override bool KeepConnectionOpen => true;

        public override void OnAcceptNewConnection(Connection connection, IAsyncResult ar)
        {
            Console.WriteLine("Game OnAcceptNewConnection");
            base.OnAcceptNewConnection(connection, ar);
            HandlerFirstConnection(connection);
        }

        public void HandlerFirstConnection(Connection connection)
        {
            connection.Send(new FirstConnectionPacket(), false);
        }

    
        public override void ProcessMessage(object sender, ConnectionEventArgs args)
        {
            args.Connection.ResetBuffer();
            var handler = _handlerFactory(args.Connection.InMessage);
            handler.HandlerMessage(args.Connection.InMessage, args.Connection);

        }
    }
}
