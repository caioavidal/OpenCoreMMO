using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;

namespace NeoServer.Server.Commands.Movements.ToInventory;

public class InventoryToInventoryOperation
{
    public static void Execute(IPlayer player, ItemThrowPacket itemThrow)
    {
        if (player.Inventory[itemThrow.FromLocation.Slot] is not IPickupable item) return;

        player.MoveItem(item,player.Inventory, player.Inventory,  itemThrow.Count,
            (byte)itemThrow.FromLocation.Slot, (byte)itemThrow.ToLocation.Slot);
    }

    public static bool IsApplicable(ItemThrowPacket itemThrowPacket)
    {
        return itemThrowPacket.FromLocation.Type == LocationType.Slot
               && itemThrowPacket.ToLocation.Type == LocationType.Slot;
    }
}