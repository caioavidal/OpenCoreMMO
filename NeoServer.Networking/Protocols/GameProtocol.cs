using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Security;
using NeoServer.Server.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Protocols
{
    public class GameProtocol : OpenTibiaProtocol
    {
        private Func<Connection, IPacketHandler> _handlerFactory;
        public GameProtocol(Func<Connection, IPacketHandler> handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }
        public override string ToString() => "Game Protocol";
        public override bool KeepConnectionOpen => true;

        public override void OnAcceptNewConnection(Connection connection, IAsyncResult ar)
        {
            Console.WriteLine("Game OnAcceptNewConnection");
            base.OnAcceptNewConnection(connection, ar);
            HandlerFirstConnection(connection);
        }

        public void HandlerFirstConnection(Connection connection)
        {
            connection.SendFirstConnection();
        }


        public override void ProcessMessage(object sender, ConnectionEventArgs args)
        {
            var connection = args.Connection;

            if (connection.IsAuthenticated && !connection.Disconnected)
            {
                Xtea.Decrypt(connection.InMessage, 6, connection.XteaKey);
            }

            var handler = _handlerFactory(args.Connection);
            handler.HandlerMessage(args.Connection.InMessage, args.Connection);
        }
    }
}
