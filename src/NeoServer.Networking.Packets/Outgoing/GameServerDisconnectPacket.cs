using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public sealed class GameServerDisconnectPacket : OutgoingPacket
    {
        private readonly string reason;
        public GameServerDisconnectPacket(string reason)
        {
            this.reason = reason;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.Disconnect);
            message.AddString(reason);
        }
    }
}
