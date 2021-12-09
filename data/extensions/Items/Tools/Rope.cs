using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items.UsableItems;
using NeoServer.Game.World.Map;

namespace NeoServer.Extensions.Items.Tools
{
    public class Rope : FloorChangerUsableItem
    {
        public Rope(IItemType metadata, Location location,  IDictionary<ItemAttribute, IConvertible> attributes) : base(metadata, location)
        {
            Console.WriteLine("oi");
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
                
                belowTile.RemoveTopItem(out var removedItem);

                usedBy.Tile.AddItem(removedItem);
                return true;
            }


            return base.Use(usedBy, item);
        }
    }
}