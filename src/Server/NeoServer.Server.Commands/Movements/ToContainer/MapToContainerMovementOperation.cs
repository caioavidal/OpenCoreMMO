using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Commands.Movements.ToContainer;

public class MapToContainerMovementOperation
{
    private readonly IItemMovementService _itemMovementService;

    public MapToContainerMovementOperation(IItemMovementService itemMovementService)
    {
        _itemMovementService = itemMovementService;
    }

    public void Execute(IPlayer player, IGameServer game, IMap map, ItemThrowPacket itemThrow)
    {
        MapToContainer(player, map, itemThrow);
    }

    private void MapToContainer(IPlayer player, IMap map, ItemThrowPacket itemThrow)
    {
        if (map[itemThrow.FromLocation] is not IDynamicTile { TopItemOnStack: IPickupable item } fromTile) return;

        var container = player.Containers[itemThrow.ToLocation.ContainerId];
        if (container is null) return;

        if (container[itemThrow.ToLocation.ContainerSlot] is IContainer innerContainer) container = innerContainer;

        _itemMovementService.Move(player, item, fromTile, container,
            itemThrow.Count, 0, (byte)itemThrow.ToLocation.ContainerSlot);
    }

    public static bool IsApplicable(ItemThrowPacket itemThrowPacket)
    {
        return itemThrowPacket.FromLocation.Type == LocationType.Ground
               && itemThrowPacket.ToLocation.Type == LocationType.Container;
    }
}