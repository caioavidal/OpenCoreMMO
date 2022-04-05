using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;

namespace NeoServer.Server.Commands.Movements.ToInventory;

public sealed class MapToInventoryMovementOperation
{
    public static void Execute(IPlayer player, IMap map, ItemThrowPacket itemThrow)
    {
        FromMapToInventory(player, map, itemThrow);
    }

    private static void FromMapToInventory(IPlayer player, IMap map, ItemThrowPacket itemThrow)
    {
        if (map[itemThrow.FromLocation] is not { } fromTile) return;
        if (fromTile.TopItemOnStack is not { } item) return;

        player.MoveItem(fromTile, player.Inventory, item, itemThrow.Count, 0, (byte)itemThrow.ToLocation.Slot);
    }

    public static bool IsApplicable(ItemThrowPacket itemThrowPacket)
    {
        return itemThrowPacket.FromLocation.Type == LocationType.Ground
               && itemThrowPacket.ToLocation.Type == LocationType.Slot;
    }
}