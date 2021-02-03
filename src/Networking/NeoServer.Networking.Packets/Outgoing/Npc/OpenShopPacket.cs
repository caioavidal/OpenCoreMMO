using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Networking.Packets.Outgoing.Npc
{
    public class OpenShopPacket : OutgoingPacket
    {
        public (IItemType, uint, uint)[] Items { get; }

        public OpenShopPacket((IItemType, uint, uint)[] items)
        {
            Items = items;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.OpenShop);

            var itemsCount = (ushort) Math.Min(Items.Length, ushort.MaxValue);
            message.AddByte((byte)itemsCount);

            foreach (var item in Items)
            {
                SendShopItem(message, item);
            }
        }

        private void SendShopItem(INetworkMessage message, (IItemType, uint, uint) itemShop)
        {
            var (item, buyPrice, sellPrice) = itemShop;

            message.AddUInt16(item.ClientId);

            //if (it.isSplash() || it.isFluidContainer())
            //{
            //    msg.addByte(serverFluidToClient(item.subType));
            //}
            //else //todo
            {
                message.AddByte(0x00);
            }

            message.AddString(item.Name);
            message.AddUInt32((uint)item.Weight * 100);
            message.AddUInt32(buyPrice);
            message.AddUInt32(sellPrice);
        }
    }
}
