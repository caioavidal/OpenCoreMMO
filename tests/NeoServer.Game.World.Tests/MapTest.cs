using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Tests;
using NeoServer.Game.World.Map.Tiles;
using NeoServer.Game.World.Tests.TestData;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NeoServer.Game.World.Tests
{
    public class MapTest
    {

        public Map.Map CreateMap(IItem item)
        {
            var world = new World();

            for (int x = 100; x < 120; x++)
            {
                for (int y = 100; y < 120; y++)
                {

                    var items = new List<IItem>()
                        {
                             ItemTestData.CreateRegularItem(1),
                        };

                    if (item.Location == new Location((ushort)x,(ushort) y, 7))
                    {
                        items.Add(item);
                    }

                    world.AddTile(new Tile(new Coordinate(x, y, 7), TileFlag.None, null, System.Array.Empty<IItem>(), items.ToArray()));
                }
            }

            return new Map.Map(world);

        }

        [Fact]
        public void RemoveThing_RemovesThingFromTile()
        {
            var item = ItemTestData.CreateMoveableItem(2);

            var map = CreateMap(item);

            var thing = item as IThing;
            map.RemoveThing(thing, map[100, 100, 7] as IDynamicTile);

            Assert.Single((map[100, 100, 7] as IDynamicTile).DownItems);
            Assert.Equal(1, (map[100, 100, 7] as IDynamicTile).DownItems.First().ClientId);
        }
    }
}
