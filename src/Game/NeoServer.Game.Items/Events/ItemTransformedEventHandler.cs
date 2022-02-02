using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;

namespace NeoServer.Game.Items.Events;

public class ItemTransformedEventHandler : IGameEventHandler
{
    private readonly IItemFactory _itemFactory;
    private readonly IMap _map;
    private readonly IMapService _mapService;

    public ItemTransformedEventHandler(IMap map, IMapService mapService, IItemFactory itemFactory)
    {
        _map = map;
        _mapService = mapService;
        _itemFactory = itemFactory;
    }

    public void Execute(IPlayer player, IItem fromItem, ushort toItem)
    {
        var createdItem = _itemFactory.Create(toItem, fromItem.Location, null);

        ReplaceItemOnGround(fromItem, createdItem);
        ReplaceGround(fromItem, createdItem);

        ReplaceItemOnContainer(player, fromItem, createdItem);
    }
    
    private static void ReplaceItemOnContainer(IPlayer player, IItem fromItem, IItem createdItem)
    {
        if (fromItem.Location.Type != LocationType.Container) return;

        var container = player.Containers[fromItem.Location.ContainerId] ?? player.Inventory?.BackpackSlot;
        
        container?.RemoveItem(fromItem, fromItem.Amount);

        if (createdItem is null) return; 

        var result = container is not null
            ? container.AddItem(createdItem)
            : new Result<OperationResult<IItem>>(InvalidOperation.NotPossible);

        if (!result.IsSuccess) player.Tile.AddItem(createdItem);
    }

    private void ReplaceGround(IItem fromItem, IItem createdItem)
    {
        if (fromItem.Location.Type != LocationType.Ground) return;
        if (_map[fromItem.Location] is not IDynamicTile tile) return;

        if (fromItem is not IGround) return;
        if (createdItem is not IGround createdGround) return;

        _mapService.ReplaceGround(fromItem.Location, createdGround);
    }

    private void ReplaceItemOnGround(IItem fromItem, IItem createdItem)
    {
        if (fromItem.Location.Type != LocationType.Ground) return;
        if (_map[fromItem.Location] is not IDynamicTile tile) return;
        if (fromItem is IGround) return;

        tile.RemoveItem(fromItem, 1, 0, out var removedThing);

        if (createdItem is null) return;
        
        tile.AddItem(createdItem);
    }
}