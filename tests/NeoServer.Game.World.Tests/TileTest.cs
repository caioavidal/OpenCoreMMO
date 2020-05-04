using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Tests;
using NeoServer.Game.World.Map.Tiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NeoServer.Game.World.Tests
{

    public class RemoveThingTileTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {

            yield return new object[] { ItemTestData.CreateCumulativeItem(500, 100), 40, 500, 60 };
            yield return new object[] { ItemTestData.CreateCumulativeItem(500, 50), 49, 500, 1 };
            yield return new object[] { ItemTestData.CreateCumulativeItem(500, 50), 1, 500, 49 };
            yield return new object[] { ItemTestData.CreateCumulativeItem(500, 1), 1, 400, 32 };
            yield return new object[] { ItemTestData.CreateCumulativeItem(500, 100), 100, 400, 32 };
            yield return new object[] { ItemTestData.CreateCumulativeItem(500, 45), 45, 400, 32 };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
    public class TileTest
    {
        private Tile CreateTile(params IItem[] item)
        {
            var items = new List<IItem> {
                ItemTestData.CreateTopItem(id: 1, topOrder: 1),
                ItemTestData.CreateRegularItem(100),
                ItemTestData.CreateRegularItem(200)
            };
            items.AddRange(item);

            var tile = new Tile(new Coordinate(100, 100, 7), TileFlag.None, items.ToArray());
            return tile;
        }


        [Fact]
        public void Constructor_Given_Items_Creates_Tile()
        {
            var tile = new Tile(new Coordinate(100, 100, 7), TileFlag.None, new List<IItem>()
            {
                ItemTestData.CreateRegularItem(id: 5),
                ItemTestData.CreateRegularItem(id: 6),
                ItemTestData.CreateRegularItem(id: 6),
                ItemTestData.CreateCumulativeItem(id: 7, amount: 35),
                ItemTestData.CreateCumulativeItem(id: 7, amount: 80),
                ItemTestData.CreateCumulativeItem(id:8, amount: 3),
                ItemTestData.CreateTopItem(id: 2, topOrder: 1),

            }.ToArray());

            var downItemsExpected = new List<IItem>
            {
                ItemTestData.CreateRegularItem(id: 5),
                ItemTestData.CreateRegularItem(id: 6),
                ItemTestData.CreateRegularItem(id: 6),
                ItemTestData.CreateCumulativeItem(id: 7, amount: 100),
                ItemTestData.CreateCumulativeItem(id: 7, amount: 15),
                ItemTestData.CreateCumulativeItem(id:8, amount: 3),
            };

            var top1Expected = new List<IItem>
            {
                ItemTestData.CreateTopItem(id: 2, topOrder: 1),
            };
            var top2Expected = new List<IItem>
            {
                ItemTestData.CreateTopItem(id: 3, topOrder: 2),
                ItemTestData.CreateTopItem(id: 4, topOrder: 2),
            };

            Assert.Collection(tile.TopItems, item => Assert.Equal(top1Expected[0].ClientId, item.ClientId));

         

            Assert.Collection(tile.DownItems, item => { Assert.Equal(downItemsExpected[5].ClientId, item.ClientId); Assert.Equal((downItemsExpected[5] as ICumulativeItem).Amount, (item as ICumulativeItem).Amount);  },
                                              item => { Assert.Equal(downItemsExpected[4].ClientId, item.ClientId); Assert.Equal((downItemsExpected[4] as ICumulativeItem).Amount, (item as ICumulativeItem).Amount); },
                                              item => { Assert.Equal(downItemsExpected[3].ClientId, item.ClientId); Assert.Equal((downItemsExpected[3] as ICumulativeItem).Amount, (item as ICumulativeItem).Amount);  },
                                              item => Assert.Equal(downItemsExpected[2].ClientId, item.ClientId),
                                              item => Assert.Equal(downItemsExpected[1].ClientId, item.ClientId),
                                              item => Assert.Equal(downItemsExpected[0].ClientId, item.ClientId));
                                         
        }

        [Fact]
        public void RemoveThing_Removes_Item_From_Stack()
        {
            var item = ItemTestData.CreateMoveableItem(500);
            var sut = CreateTile(item);

            var thing = (IMoveableThing)item;
            sut.RemoveThing(ref thing);

            Assert.Equal(2, sut.DownItems.Count);
            Assert.Single(sut.TopItems);

            Assert.Equal(200, sut.DownItems.First().ClientId);
        }
        [Theory]
        [ClassData(typeof(RemoveThingTileTestData))]
        public void RemoveThing_Removes_CumulativeItem_From_Stack(ICumulativeItem item, byte amountToRemove, ushort topItemId, byte remainingAmount)
        {
            var item2 = ItemTestData.CreateCumulativeItem(400, 32);
            var sut = CreateTile(item2, item);

            var thing = (IMoveableThing)item;
            sut.RemoveThing(ref thing, amountToRemove);

            Assert.Equal(topItemId, sut.DownItems.First().ClientId);
            Assert.Equal(remainingAmount, (sut.DownItems.First() as ICumulativeItem).Amount);
        }
    }


}
