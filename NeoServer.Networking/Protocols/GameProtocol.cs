using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Security;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Protocols
{
    public class GameProtocol : OpenTibiaProtocol
    {
        private Func<IConnection, IPacketHandler> _handlerFactory;
        public GameProtocol(Func<IConnection, IPacketHandler> handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }
        public override string ToString() => "Game Protocol";
        public override bool KeepConnectionOpen => true;

        public override void OnAcceptNewConnection(IConnection connection)
        {
            Console.WriteLine("Game OnAcceptNewConnection");
            HandlerFirstConnection(connection);
            //base.OnAcceptNewConnection(connection);
            
        }

        public void HandlerFirstConnection(IConnection connection)
        {
            connection.SendFirstConnection();
        }


        public override void ProcessMessage(object sender, IConnectionEventArgs args)
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
