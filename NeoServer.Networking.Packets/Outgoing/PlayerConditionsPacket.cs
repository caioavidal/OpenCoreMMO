using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerConditionsPacket:OutgoingPacket
    {
        public PlayerConditionsPacket() : base(false)
        {
            OutputMessage.AddByte((byte)GameOutgoingPacketType.PlayerConditions);
            OutputMessage.AddUInt16(0x00);
        }
    }
}
