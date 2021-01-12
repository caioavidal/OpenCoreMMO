using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.World.Map;
using NeoServer.Game.World.Map.Tiles;

namespace NeoServer.Game.Tests.Helpers
{
    public class MapTestDataBuilder
    {
        public static Map CreateMap(int fromX, int toX, int fromY, int toY, int fromZ, int toZ)
        {
            var world = new World.World();

            for (int x = fromX; x < toX; x++)
            {
                for (int y = fromY; y < toY; y++)
                {
                    for (int z = fromZ; z < toZ; z++)
                    {
                        world.AddTile(new Tile(new Coordinate(x, y, 7), TileFlag.None, null, new IItem[0], null));
                    }

                }
            }

            return new Map(world);

        }
    }
}
