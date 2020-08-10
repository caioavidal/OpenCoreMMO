using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class CancelTargetPacket : OutgoingPacket
    {

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.CancelTarget);
            message.AddByte(0x00);
        }
    }
}
