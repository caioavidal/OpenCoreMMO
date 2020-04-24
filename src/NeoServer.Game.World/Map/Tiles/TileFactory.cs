using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.World.Map.Tiles
{
    public class TileFactory
    {
        //public static ITile Build(List<IItem> items, Coordinate coordinate)
        //{

        //}

        public static ITile CreateTile(Coordinate coordinate, TileFlag flag, IItem[] items)
        {

            var hasUnpassableItem = false;
            var hasMoveableItem = false;

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

            }

            if (hasUnpassableItem && !hasMoveableItem)
            {
                return new ImmutableTile(coordinate, items);
            }


            return new Tile(coordinate, flag, items);
        }
    }
}


