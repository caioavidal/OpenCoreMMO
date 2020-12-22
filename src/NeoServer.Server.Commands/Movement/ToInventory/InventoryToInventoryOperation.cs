using NeoServer.Game.Common.Location;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Commands.Movement.ToInventory
{
    public class InventoryToInventoryOperation
    {
        public static void Execute(IPlayer player, ItemThrowPacket itemThrow)
        {
            if (player.Inventory[itemThrow.FromLocation.Slot] is not IPickupable item) return;

            if (player.Inventory.CanAddItemToSlot(itemThrow.ToLocation.Slot, item).IsSuccess is false) return;

            if (player.Inventory.RemoveItemFromSlot(itemThrow.FromLocation.Slot, itemThrow.Count, out var removedItem) is false) return;

            player.Inventory.TryAddItemToSlot(itemThrow.ToLocation.Slot, removedItem);
        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) =>
          itemThrowPacket.FromLocation.Type == LocationType.Slot
          && itemThrowPacket.ToLocation.Type == LocationType.Slot;
    }
}
