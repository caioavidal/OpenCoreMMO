using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;

namespace NeoServer.Application.Features.Movement.Strategies;

public class FromMapToContainerMovement : IItemMovement
{
    private readonly IItemMovementService _itemMovementService;
    private readonly IMap _map;

    public FromMapToContainerMovement(IItemMovementService itemMovementService, IMap map)
    {
        _itemMovementService = itemMovementService;
        _map = map;
    }

    public void Handle(IPlayer player, ItemThrowPacket itemThrow)
    {
        MapToContainer(player, _map, itemThrow);
    }

    public string MovementKey => $"{LocationType.Ground.ToString()}-{LocationType.Container.ToString()}";

    private void MapToContainer(IPlayer player, IMap map, ItemThrowPacket itemThrow)
    {
        var tile = map[itemThrow.FromLocation];

        if (tile is not IDynamicTile fromTile) return;
        var item = fromTile.TopItemOnStack;

        if (item is null) return;
        if (!item.IsPickupable) return;

        var container = player.Containers[itemThrow.ToLocation.ContainerId];
        if (container is null) return;

        _itemMovementService.Move(player, item, fromTile, container,
            itemThrow.Count, 0, (byte)itemThrow.ToLocation.ContainerSlot);
    }
}