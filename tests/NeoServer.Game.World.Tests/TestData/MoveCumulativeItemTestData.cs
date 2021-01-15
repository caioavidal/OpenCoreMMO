using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Items.Tests;
using NeoServer.Game.World.Map.Tiles;
using System.Collections;
using System.Collections.Generic;

namespace NeoServer.Game.World.Tests.TestData
{


    public class MoveCumulativeItemTestData : IEnumerable<object[]>
    {
        public class Data
        {
            public IMap Map { get; set; }
            public ICumulative Item { get; set; }
            public byte Amount { get; set; }
            public Location ToLocation { get; set; }
            public List<IItem> ExpectedFromTileDowmItems { get; set; }
            public List<IItem> ExpectedToTileDowmItems { get; set; }

            public Data(ICumulative item, byte amount, Location toLocation, List<IItem> expectedFromTileDowmItems, List<IItem> expectedToTileDowmItems)
            {
                Map = CreateMap(item);
                Item = item;
                Amount = amount;
                ToLocation = toLocation;
                ExpectedFromTileDowmItems = expectedFromTileDowmItems;
                ExpectedToTileDowmItems = expectedToTileDowmItems;
            }
           
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

                        if (item.Location == new Location((ushort)x, (ushort)y, 7))
                        {
                            items.Add(item);
                        }

                        world.AddTile(new Tile(new Coordinate(x, y, 7), TileFlag.None, null, new IItem[0], items.ToArray()));
                    }
                }

                return new Map.Map(world);

            }
        }
        public IEnumerator<object[]> GetEnumerator()
        {

            yield return new object[] { new Data(ItemTestData.CreateCumulativeItem(5, 100),amount:40, toLocation: new Location(101, 100, 7),
                expectedFromTileDowmItems: new List<IItem>(){

                }, expectedToTileDowmItems: new List<IItem>())};

        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }

}
