using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Networking.Packets.Incoming.Shop
{
    public class PlayerSalePacket : IncomingPacket
    {
        public ushort ItemClientId { get;  }
        public byte Count { get; }
        public byte Amount { get; set; }
        public bool IgnoreEquipped { get; set; }

        public PlayerSalePacket(IReadOnlyNetworkMessage message)
        {
            ItemClientId = message.GetUInt16();
            Count = message.GetByte();
            Amount = message.GetByte();
            IgnoreEquipped = message.GetByte() != 0;
        }
    }
}
