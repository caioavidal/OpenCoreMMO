using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
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

                    if (item.Location == new Location(x, y, 7))
                    {
                        items.Add(item);
                    }

                    world.AddTile(new Tile(new Coordinate(x, y, 7), TileFlag.None, null, new IItem[0], items.ToArray()));
                }
            }

            return new Map.Map(world);

        }

        [Fact]
        public void RemoveThing_RemovesThingFromTile()
        {
            var item = ItemTestData.CreateMoveableItem(2);

            var map = CreateMap(item);

            var thing = item as IMoveableThing;
            map.RemoveThing(ref thing, map[100, 100, 7] as IWalkableTile);

            Assert.Single((map[100, 100, 7] as IWalkableTile).DownItems);
            Assert.Equal(1, (map[100, 100, 7] as IWalkableTile).DownItems.First().ClientId);
        }

        [Fact]
        public void TryMoveThing_Moves_Thing()
        {
            var item = ItemTestData.CreateMoveableItem(2);

            var sup = CreateMap(item);
            var fromTile = sup[item.Location] as IWalkableTile;

            var thing = item as IMoveableThing;

            var result = sup.TryMoveThing(ref thing, new Location(101, 100, 7));

            Assert.True(result);
            Assert.Equal(new Location(101, 100, 7), thing.Location);

            Assert.Collection(fromTile.DownItems, item => Assert.Equal(1, item.ClientId));
            Assert.Collection((sup[thing.Location] as IWalkableTile).DownItems, downItem => Assert.Same(item, downItem),
                item => Assert.Equal(1, item.ClientId));

        }

        [Fact]
        public void TryMoveThing_GivenRegularItem_Moves_Thing()
        {
            var item = ItemTestData.CreateMoveableItem(2);

            var sup = CreateMap(item);
            var fromTile = sup[item.Location] as IWalkableTile;

            var thing = item as IMoveableThing;

            var result = sup.TryMoveThing(ref thing, new Location(101, 100, 7));

            Assert.True(result);
            Assert.Equal(new Location(101, 100, 7), thing.Location);

            Assert.Collection(fromTile.DownItems, item => Assert.Equal(1, item.ClientId));
            Assert.Collection((sup[thing.Location] as IWalkableTile).DownItems, downItem => Assert.Same(item, downItem),
                item => Assert.Equal(1, item.ClientId));

        }
        [Theory]
        [ClassData(typeof(MoveCumulativeItemTestData))]
        public void TryMoveThing_GivenCumulativeItem_Moves_Thing(MoveCumulativeItemTestData.Data data)
        {
            var item = ItemTestData.CreateMoveableItem(2);

            var sup = CreateMap(item);
            var fromTile = sup[item.Location] as IWalkableTile;

            var thing = item as IMoveableThing;

            var result = sup.TryMoveThing(ref thing, new Location(101, 100, 7));

            Assert.True(result);
            Assert.Equal(new Location(101, 100, 7), thing.Location);

            Assert.Collection(fromTile.DownItems, item => Assert.Equal(1, item.ClientId));
            Assert.Collection((sup[thing.Location] as IWalkableTile).DownItems, downItem => Assert.Same(item, downItem),
                item => Assert.Equal(1, item.ClientId));

        }

    }
}
