using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Helpers;

namespace NeoServer.Scripts.Lua.Functions;

public static class TileFunctions
{
    public static void AddTileFunctions(this NLua.Lua lua)
    {
        lua.DoString("tile_helper = {}");
        lua["tile_helper.removeTopItem"] = RemoveTopItem;
        lua["tile_helper.addItem"] = AddItem;
    }

    private static bool RemoveTopItem(Location location)
    {
        var map = IoC.GetInstance<IMap>();
        var tile = map.GetTile(location);

        var staticToDynamicTileService = IoC.GetInstance<IStaticToDynamicTileService>();

        var dynamicTile = staticToDynamicTileService.TransformIntoDynamicTile(tile) as IDynamicTile;

        return dynamicTile?.RemoveTopItem(force:true).Succeeded ?? false; 
    }
    
    private static bool AddItem(Location location, ushort itemId, byte amount = 1)
    {
        var map = IoC.GetInstance<IMap>();
        var tile = map.GetTile(location);
        
        var staticToDynamicTileService = IoC.GetInstance<IStaticToDynamicTileService>();

        var dynamicTile = staticToDynamicTileService.TransformIntoDynamicTile(tile) as IDynamicTile;

        var itemFactory = IoC.GetInstance<IItemFactory>();
        var item = itemFactory.Create(itemId,location, new Dictionary<ItemAttribute, IConvertible>()
        {
            [ItemAttribute.Count] = amount
        });

        return dynamicTile?.AddItem(item).Succeeded ?? false; 
    }
}