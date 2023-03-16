using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Items.Services.ItemTransform.Operations;

internal static class ReplaceItemFromGroundOperation
{
    public static Result<IItem> Execute(IMap map, IItemFactory itemFactory, IItem fromItem, IItemType toItemType)
    {
        if (fromItem.Location.Type != LocationType.Ground) return Result<IItem>.NotApplicable;
        if (map[fromItem.Location] is not IDynamicTile tile) return Result<IItem>.NotApplicable;
        if (fromItem is IGround) return Result<IItem>.NotApplicable;

        if (toItemType is null) fromItem.MarkAsDeleted();

        var result = tile.UpdateItemType(fromItem, toItemType);
        if (result) return Result<IItem>.Ok(fromItem);

        var createdItem = toItemType is null ? null : itemFactory.Create(toItemType, fromItem.Location, null);

        tile.ReplaceItem(fromItem, createdItem);
        return Result<IItem>.Ok(createdItem);
    }
}