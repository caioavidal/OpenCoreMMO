using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public sealed class GameServerDisconnectPacket : OutgoingPacket
    {
        public GameServerDisconnectPacket(string reason)
        {
            OutputMessage.AddByte((byte)GameOutgoingPacketType.Disconnect);
            OutputMessage.AddString(reason);
        }

        public override bool Disconnect { get; protected set; } = true;
    }
}
