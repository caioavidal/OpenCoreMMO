using System;
using NeoServer.Networking.Handlers;
using NeoServer.Networking.Packets.Security;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Protocols
{
    public class GameProtocol : Protocol
    {
        private readonly Func<IConnection, IPacketHandler> _handlerFactory;

        public GameProtocol(Func<IConnection, IPacketHandler> handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        public override bool KeepConnectionOpen => true;

        public override string ToString()
        {
            return "Game Protocol";
        }

        public override void OnAccept(IConnection connection)
        {
            HandlerFirstConnection(connection);
            base.OnAccept(connection);
        }

        public void HandlerFirstConnection(IConnection connection)
        {
            connection.SendFirstConnection();
        }

        public override void ProcessMessage(object sender, IConnectionEventArgs args)
        {
            var connection = args.Connection;

            if (connection.IsAuthenticated && !connection.Disconnected)
                Xtea.Decrypt(connection.InMessage, 6, connection.XteaKey);

            if (_handlerFactory(args.Connection) is not IPacketHandler handler) return;

            handler?.HandlerMessage(args.Connection.InMessage, args.Connection);
        }
    }
}