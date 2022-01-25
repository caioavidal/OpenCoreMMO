using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures;
using NeoServer.Game.Items.Items.UsableItems;
using NeoServer.Game.World.Map;
using NeoServer.Game.World.Services;

namespace NeoServer.Extensions.Items.Tools
{
    public class Rope : FloorChangerUsableItem
    {
        public Rope(IItemType metadata, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(
            metadata, location)
        {
        }

        public override bool Use(ICreature usedBy, IItem item)
        {
            if (Map.Instance[item.Location] is not IDynamicTile tile) return false;

            item = tile.Ground;

            if (item.Metadata.Attributes.TryGetAttribute(ItemAttribute.FloorChange, out var floorChange) &&
                floorChange == "down")
                return PullThing(usedBy, item, tile);

            return base.Use(usedBy, item);
        }

        private static bool PullThing(ICreature usedBy, IItem item, IDynamicTile tile)
        {
            var belowFloor = item.Location.AddFloors(1);

            if (Map.Instance[belowFloor] is not IDynamicTile belowTile) return false;

            if (belowTile.Players.LastOrDefault() is { } player) return PullCreature(tile, player);

            return PullItem(usedBy, belowTile);
        }

        private static bool PullItem(ICreature usedBy, IDynamicTile belowTile)
        {
            if (!belowTile.RemoveTopItem(out var removedItem)) return false;

            usedBy.Tile.AddItem(removedItem);
            return true;
        }

        private static bool PullCreature(IDynamicTile tile, IPlayer player)
        {
            var found = MapService
                .Instance
                .GetNeighbourAvailableTile(tile.Location, player, PlayerEnterTileRule.Rule, out var destinationTile);

            if (!found) return false;

            Map.Instance.TryMoveCreature(player, destinationTile.Location);
            return true;
        }
    }
}