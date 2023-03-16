using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Items.Services.ItemTransform.Operations;

internal static class ReplaceGroundOperation
{
    public static Result<IItem> Execute(IMap map, IMapService mapService, IItem fromItem, IItem createdItem)
    {
        if (fromItem.Location.Type != LocationType.Ground) return Result<IItem>.NotApplicable;
        if (map[fromItem.Location] is not IDynamicTile) return Result<IItem>.NotApplicable;

        if (fromItem is not IGround) return Result<IItem>.NotApplicable;
        if (createdItem is not IGround createdGround) return Result<IItem>.NotApplicable;

        mapService.ReplaceGround(fromItem.Location, createdGround);
        return Result<IItem>.Ok(createdGround);
    }
}