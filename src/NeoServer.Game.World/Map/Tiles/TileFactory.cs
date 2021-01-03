using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using System.Collections.Generic;

namespace NeoServer.Game.World.Map.Tiles
{
    public class TileFactory
    {
        public static ITile CreateTile(Coordinate coordinate, TileFlag flag, IItem[] items)
        {

            var hasUnpassableItem = false;
            var hasMoveableItem = false;
            IGround ground = null;

            var topItems = new List<IItem>();
            var downItems = new List<IItem>();

            foreach (var item in items)
            {
                if (item.IsBlockeable)
                {
                    hasUnpassableItem = true;
                }

                if (item.CanBeMoved)
                {
                    hasMoveableItem = true;
                }

                if (item.IsAlwaysOnTop)
                {
                    topItems.Add(item);
                    continue;
                }

                if (item is IGround)
                {
                    ground = item as IGround;
                    continue;
                }

                downItems.Add(item);
            }

            if (hasUnpassableItem && !hasMoveableItem)
            {
                return new StaticTile(coordinate, items);
            }

            return new Tile( coordinate, flag, ground, topItems.ToArray(), downItems.ToArray());
        }
    }
}

