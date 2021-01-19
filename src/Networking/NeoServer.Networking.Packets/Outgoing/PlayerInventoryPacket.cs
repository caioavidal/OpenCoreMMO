using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;
using System;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerInventoryPacket : OutgoingPacket
    {
        private readonly IInventory inventory;
        public PlayerInventoryPacket(IInventory inventory)
        {
            this.inventory = inventory;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            var addInventoryItem = new Action<Slot>(slot =>
          {
              if (inventory[slot] == null)
              {
                  message.AddByte((byte)GameOutgoingPacketType.InventoryEmpty);
                  message.AddByte((byte)slot);
              }
              else
              {
                  message.AddByte((byte)GameOutgoingPacketType.InventoryItem);
                  message.AddByte((byte)slot);
                  message.AddItem(inventory[slot]);
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
