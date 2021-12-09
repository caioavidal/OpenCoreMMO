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
        public Rope(IItemType metadata, Location location,  IDictionary<ItemAttribute, IConvertible> attributes) : base(metadata, location)
        {
        }

        public override bool Use(ICreature usedBy, IItem item)
        {
            if (Map.Instance[item.Location] is not IDynamicTile tile) return false;

            item = tile.Ground;
            
            if (item.Metadata.Attributes.TryGetAttribute(ItemAttribute.FloorChange, out var floorChange) &&
                floorChange == "down")
            {
                var belowFloor = item.Location.AddFloors(1);

                if (Map.Instance[belowFloor] is not IDynamicTile belowTile) return false;

                if (belowTile.Players.LastOrDefault() is {} player)
                {
                    var found = MapService
                        .Instance
                        .GetNeighbourAvailableTile(tile.Location, player, PlayerEnterTileRule.Rule, out var destinationTile);

                    if (!found) return false;
                    
                    Map.Instance.TryMoveCreature(player, destinationTile.Location);
                    return true;
                }
                
                if (!belowTile.RemoveTopItem(out var removedItem)) return false;

                usedBy.Tile.AddItem(removedItem);
                return true;
            }


            return base.Use(usedBy, item);
        }
    }
}