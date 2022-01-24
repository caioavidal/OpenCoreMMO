using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Npc;

public class OpenShopPacket : OutgoingPacket
{
    public OpenShopPacket(IEnumerable<IShopItem> items)
    {
        Items = items;
    }

    public IEnumerable<IShopItem> Items { get; }

    public override void WriteToMessage(INetworkMessage message)
    {
        message.AddByte((byte)GameOutgoingPacketType.OpenShop);

        var itemsCount = (ushort)Math.Min(Items.Count(), ushort.MaxValue);
        message.AddByte((byte)itemsCount);

        foreach (var item in Items) SendShopItem(message, item);
    }

    private void SendShopItem(INetworkMessage message, IShopItem shopItem)
    {
        if (shopItem is null) return;

        message.AddUInt16(shopItem.Item.ClientId);

        //if (it.isSplash() || it.isFluidContainer())
        //{
        //    msg.addByte(serverFluidToClient(item.subType));
        //}
        //else //todo
        {
            message.AddByte(0x00);
        }

        message.AddString(shopItem.Item.Name);
        message.AddUInt32((uint)shopItem.Item.Weight * 100);
        message.AddUInt32(shopItem.BuyPrice);
        message.AddUInt32(shopItem.SellPrice);
    }
}