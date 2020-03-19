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
        private Func<IReadOnlyNetworkMessage, IncomingPacket> _packetFactory;

        public GameProtocol(Func<IReadOnlyNetworkMessage, IncomingPacket> packetFactory)
        {
            _packetFactory = packetFactory;
        }

        public override bool KeepConnectionOpen => true;

        public override void OnAcceptNewConnection(Connection connection, IAsyncResult ar)
        {
            //throw new NotImplementedException();
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
            var packet = _packetFactory(args.Connection.InMessage);
            args.Connection.SetXtea(packet.Xtea);

            var eventArgs = new ServerEventArgs(packet.Model, args.Connection, packet.SuccessFunc);

            packet.OnIncomingPacket(args.Connection, eventArgs);
        }
    }
}
