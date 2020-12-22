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

            player.MoveThing(player.Inventory, player.Inventory, item, itemThrow.Count, (byte)itemThrow.FromLocation.Slot, (byte)itemThrow.ToLocation.Slot);
        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) =>
          itemThrowPacket.FromLocation.Type == LocationType.Slot
          && itemThrowPacket.ToLocation.Type == LocationType.Slot;
    }
}
