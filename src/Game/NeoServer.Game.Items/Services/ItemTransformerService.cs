using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Services;

public class ItemTransformerService : IItemTransformerService
{
    private readonly IMap _map;
    private readonly IItemFactory _itemFactory;
    private readonly ITileFactory _tileFactory;
    private readonly IItemClientServerIdMapStore _itemClientServerIdMapStore;

    public ItemTransformerService(IMap map, IItemFactory itemFactory , ITileFactory tileFactory, IItemClientServerIdMapStore itemClientServerIdMapStore)
    {
        _map = map;
        _itemFactory = itemFactory;
        _tileFactory = tileFactory;
        _itemClientServerIdMapStore = itemClientServerIdMapStore;
    }

    public IItem Transform(ITile tile, ushort fromItemId, ushort toItemId)
    {
        if (tile is null) return null;

        tile= TransformIntoDynamicTile(tile);

        if (tile is not IDynamicTile dynamicTile) return null;

        var newItem = _itemFactory.Create(toItemId, tile.Location, new Dictionary<ItemAttribute, IConvertible>());
        
        dynamicTile.ReplaceItem(fromItemId, newItem);

        return newItem;
    }

    private ITile TransformIntoDynamicTile(ITile tile)
    {
        if (tile is not IStaticTile staticTile) return tile;
        
        var itemsId = staticTile.AllClientIdItems;

        var items = new List<IItem>(itemsId.Length);

        foreach (var clientId in itemsId)
        {
            if (!_itemClientServerIdMapStore.TryGetValue(clientId, out var serverId)) continue;

            var item = _itemFactory.Create(serverId, tile.Location, new Dictionary<ItemAttribute, IConvertible>());
            items.Add(item);
        }

        var dynamicTile = _tileFactory.CreateDynamicTile(new Coordinate(tile.Location), TileFlag.None, items.ToArray());

        _map.ReplaceTile(dynamicTile);
        return dynamicTile;
    }
}