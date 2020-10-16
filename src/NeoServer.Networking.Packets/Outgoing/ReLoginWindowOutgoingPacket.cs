using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class ReLoginWindowOutgoingPacket : OutgoingPacket
    {
       
        public ReLoginWindowOutgoingPacket()
        {
        }

        public override void WriteToMessage(INetworkMessage message) => message.AddByte((byte)GameOutgoingPacketType.ReLoginWindow);
        
    }
}
