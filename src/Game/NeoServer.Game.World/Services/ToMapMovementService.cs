using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures.Structs;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Texts;
using NeoServer.Game.World.Algorithms;
using NeoServer.Game.World.Models.Tiles;

namespace NeoServer.Game.World.Services;

public class ToMapMovementService : IToMapMovementService
{
    private readonly IMap _map;
    private readonly IMapService _mapService;

    public ToMapMovementService(IMap map, IMapService mapService)
    {
        _map = map;
        _mapService = mapService;
    }

    public void Move(IPlayer player, MovementParams itemThrow)
    {
        var finalTile = _mapService.GetFinalTile(itemThrow.ToLocation);

        if (finalTile is not IDynamicTile)
        {
            OperationFailService.Display(player.CreatureId, TextConstants.NOT_ENOUGH_ROOM);
            return;
        }

        if (!SightClear.IsSightClear(_map, player.Location, itemThrow.ToLocation, false))
        {
            OperationFailService.Display(player.CreatureId, TextConstants.YOU_CANNOT_THROW_THERE);
            return;
        }

        FromGround(player, itemThrow);
        FromInventory(player, itemThrow);
        FromContainer(player, itemThrow);
    }

    private void FromGround(IPlayer player, MovementParams movementParams)
    {
        if (movementParams.FromLocation.Type != LocationType.Ground) return;

        if (_map[movementParams.FromLocation] is not DynamicTile fromTile) return;
        if (_map[movementParams.ToLocation] is not DynamicTile toTile) return;

        if (fromTile.TopItemOnStack is not { } item) return;

        var finalTile = (DynamicTile)_mapService.GetFinalTile(toTile.Location);

        player.MoveItem(fromTile, finalTile, item, movementParams.Amount, 0, 0);
    }

    private void FromInventory(IPlayer player, MovementParams movementParams)
    {
        if (movementParams.FromLocation.Type is not LocationType.Slot) return;
        if (_map[movementParams.ToLocation] is not IDynamicTile toTile) return;
        if (player.Inventory[movementParams.FromLocation.Slot] is not IPickupable item) return;

        var finalTile = (DynamicTile)_mapService.GetFinalTile(toTile.Location);

        player.MoveItem(player.Inventory, finalTile, item, movementParams.Amount,
            (byte)movementParams.FromLocation.Slot, 0);
    }

    private void FromContainer(IPlayer player, MovementParams itemThrow)
    {
        if (itemThrow.FromLocation.Type is not LocationType.Container) return;
        if (_map[itemThrow.ToLocation] is not IDynamicTile toTile) return;

        var container = player.Containers[itemThrow.FromLocation.ContainerId];
        if (container[itemThrow.FromLocation.ContainerSlot] is not IPickupable item) return;

        var finalTile = (DynamicTile)_mapService.GetFinalTile(toTile.Location);

        player.MoveItem(container, finalTile, item, itemThrow.Amount, (byte)itemThrow.FromLocation.ContainerSlot, 0);
    }
}