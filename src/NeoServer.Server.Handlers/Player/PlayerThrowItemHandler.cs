using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerThrowItemHandler : PacketHandler
    {
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var itemThrowPacket = new ItemThrowPacket(message);


        }
    }
}
