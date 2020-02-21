using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Incoming;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Protocols
{
    public class LoginProtocol : OpenTibiaProtocol
    {
        public override bool KeepConnectionOpen => false;
        public LoginProtocol()
        {
        }
        public override void ProcessMessage(object sender, ConnectionEventArgs args)
        {
            var handler = HandlerFactory.GetHandler(GameIncomingPacketType.AddVip);
            var packet = (PacketIncoming)Activator.CreateInstance(handler.IncomingPacket, args.Connection.InMessage);

            handler.EventHandler.Handler(args.Connection, packet.Model);
        }
    }
}
