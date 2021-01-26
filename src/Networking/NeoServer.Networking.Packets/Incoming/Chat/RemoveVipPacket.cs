using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Networking.Packets.Incoming.Chat
{
    public class RemoveVipPacket : IncomingPacket
    {
        public uint PlayerId { get; set; }
        public RemoveVipPacket(IReadOnlyNetworkMessage message)
        {
            PlayerId = message.GetUInt32();
        }
    }
}
