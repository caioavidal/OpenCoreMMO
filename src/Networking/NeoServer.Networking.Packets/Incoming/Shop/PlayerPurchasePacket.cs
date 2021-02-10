using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Networking.Packets.Incoming.Shop
{
    public class PlayerPurchasePacket : IncomingPacket
    {
        public ushort ItemClientId { get;  }
        public byte Count { get; }
        public byte Amount { get; set; }
        public bool IgnoreCapacity { get; set; }
        public bool InBackpacks { get; set; }

        public PlayerPurchasePacket(IReadOnlyNetworkMessage message)
        {
            ItemClientId = message.GetUInt16();
            Count = message.GetByte();
            Amount = message.GetByte();
            IgnoreCapacity = message.GetByte() != 0;
            InBackpacks = message.GetByte() != 0;
        }
    }
}
