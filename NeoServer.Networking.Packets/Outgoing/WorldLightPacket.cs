using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class WorldLightPacket:OutgoingPacket
    {
        public WorldLightPacket(byte Level, byte Color) : base(false)
        {
            OutputMessage.AddByte((byte)GameOutgoingPacketType.WorldLight);

            OutputMessage.AddByte(Level);
            OutputMessage.AddByte(Color);
        }
    }
}
