using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Common.Texts;
using NeoServer.Game.Creature;
using NeoServer.Game.Item.Items.UsableItems;
using NeoServer.Game.World.Map;
using NeoServer.Game.World.Services;

namespace NeoServer.Extensions.Items.Tools;

public class Rope : FloorChangerUsableItem, IUsableOnItem
{
    public Rope(IItemType metadata, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(
        metadata, location)
    {
    }

    public new Result Use(ICreature usedBy, IItem onItem)
    {
        if (!CanUse(usedBy, onItem))
        {
            OperationFailService.Send(usedBy.CreatureId, TextConstants.NOT_POSSIBLE);
            return Result.NotPossible;
        }

        if (Map.Instance[onItem.Location] is not IDynamicTile tile) return Result.Fail(InvalidOperation.NotEnoughRoom);

        onItem = tile.Ground;

        if (onItem.Metadata.Attributes.TryGetAttribute(ItemAttribute.FloorChange, out var floorChange) &&
            floorChange == "down")
            return PullThing(usedBy, onItem, tile);

        return base.Use(usedBy, onItem);
    }

    private static Result PullThing(ICreature usedBy, IItem item, IDynamicTile tile)
    {
        var belowFloor = item.Location.AddFloors(1);

        if (Map.Instance[belowFloor] is not IDynamicTile belowTile) return Result.Fail(InvalidOperation.NotEnoughRoom);

        if (belowTile.Players.LastOrDefault() is { } player) return PullCreature(tile, player);

        return PullItem(usedBy, belowTile);
    }

    private static Result PullItem(ICreature usedBy, IDynamicTile belowTile)
    {
        var result = belowTile.RemoveTopItem();
        if (result.Failed) return Result.NotApplicable;

        usedBy.Tile.AddItem(result.Value);
        return Result.Success;
    }

    private static Result PullCreature(IDynamicTile tile, IPlayer player)
    {
        var found = MapService
            .Instance
            .GetNeighbourAvailableTile(tile.Location, player, PlayerEnterTileRule.Rule, out var destinationTile);

        if (!found) return Result.NotApplicable;

        Map.Instance.TryMoveCreature(player, destinationTile.Location);
        return Result.Success;
    }
}