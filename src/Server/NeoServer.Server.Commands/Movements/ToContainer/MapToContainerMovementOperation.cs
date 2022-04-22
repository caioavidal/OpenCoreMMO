using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Commands.Movements.ToContainer;

public class MapToContainerMovementOperation
{
    public static void Execute(IPlayer player, IGameServer game, IMap map, ItemThrowPacket itemThrow)
    {
        MapToContainer(player, map, itemThrow);
    }

    private static void MapToContainer(IPlayer player, IMap map, ItemThrowPacket itemThrow)
    {
        if (map[itemThrow.FromLocation] is not IDynamicTile fromTile) return;

        if (fromTile.TopItemOnStack is not IPickupable item) return;

        if (!itemThrow.FromLocation.IsNextTo(player.Location)) player.WalkTo(itemThrow.FromLocation);

        var container = player.Containers[itemThrow.ToLocation.ContainerId];
        if (container is null) return;

        if (container[itemThrow.ToLocation.ContainerSlot] is IContainer innerContainer) container = innerContainer;

        player.MoveItem(item,fromTile, container,  itemThrow.Count, 0, (byte)itemThrow.ToLocation.ContainerSlot);
    }

    public static bool IsApplicable(ItemThrowPacket itemThrowPacket)
    {
        return itemThrowPacket.FromLocation.Type == LocationType.Ground
               && itemThrowPacket.ToLocation.Type == LocationType.Container;
    }
}