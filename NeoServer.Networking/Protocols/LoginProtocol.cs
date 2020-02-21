using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Handlers;
using NeoServer.Server.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Protocols
{
    public class LoginProtocol : OpenTibiaProtocol
    {
        public override bool KeepConnectionOpen => false;
        private Func<GameIncomingPacketType, IEventHandler> _handlerFactory;
        public LoginProtocol()
        {
        }

        public override void ProcessMessage(object sender, ConnectionEventArgs args)
        {

            var handler = _handlerFactory(GameIncomingPacketType.AddVip);
            //var handler = HandlerFactory.GetHandler(GameIncomingPacketType.AddVip);
            //var packet = (IncomingPacket)Activator.CreateInstance(handler.IncomingPacket, args.Connection.InMessage);

            //handler.EventHandler.Handler(args.Connection, packet.Model);
        }
    }
}
