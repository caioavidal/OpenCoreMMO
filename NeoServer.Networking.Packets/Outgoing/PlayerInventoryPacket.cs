using NeoServer.Server.Model.Creatures.Contracts;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerInventoryPacket:OutgoingPacket
    {
        public PlayerInventoryPacket(IInventory inventory) : base(false)
        {
            var addInventoryItem = new Action<Slot>(slot =>
            {
                if (inventory[(byte)slot] == null)
                {
                    OutputMessage.AddByte((byte)GameOutgoingPacketType.InventoryEmpty);
                    OutputMessage.AddByte((byte)slot);
                }
                else
                {
                    OutputMessage.AddByte((byte)GameOutgoingPacketType.InventoryItem);
                    OutputMessage.AddByte((byte)slot);
                    OutputMessage.AddItem(inventory[(byte)slot]);
                }
            });

            addInventoryItem(Slot.Head);
            addInventoryItem(Slot.Necklace);
            addInventoryItem(Slot.Backpack);
            addInventoryItem(Slot.Body);
            addInventoryItem(Slot.Right);
            addInventoryItem(Slot.Left);
            addInventoryItem(Slot.Legs);
            addInventoryItem(Slot.Feet);
            addInventoryItem(Slot.Ring);
            addInventoryItem(Slot.Ammo);
        }
    }
}
