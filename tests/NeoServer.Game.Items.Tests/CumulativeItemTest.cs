using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Items.Items;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NeoServer.Game.Items.Tests
{
    public class CumulativeItemTest
    {
        [Fact]
        public void TryJoin_When_Sum_Of_Amount_Less_Than_100_Outs_Null()
        {
            var type = new ItemType();
            type.SetClientId(100);

            var sup = new CumulativeItem(type, new Location(100, 100, 7), 50);

            ICumulativeItem itemToJoin = new CumulativeItem(type, new Location(100, 100, 7), 40);
            sup.TryJoin(ref itemToJoin);

            Assert.Null(itemToJoin);
            Assert.Equal(90, sup.Amount);
        }
        [Fact]
        public void TryJoin_When_Sum_Of_Amount_Bigger_Than_100_Outs_Item_With_Exceeding_Amount()
        {
            var type = new ItemType();
            type.SetClientId(100);

            var sup = new CumulativeItem(type, new Location(100, 100, 7), 50);

            ICumulativeItem itemToJoin = new CumulativeItem(type, new Location(100, 100, 7), 70);
            sup.TryJoin(ref itemToJoin);

            Assert.Equal(20, itemToJoin.Amount);
            Assert.Equal(100, sup.Amount);
        }
        [Fact]
        public void TryJoin_When_Item_To_Join_Has_Amount_100()
        {
            var type = new ItemType();
            type.SetClientId(100);

            var sup = new CumulativeItem(type, new Location(100, 100, 7), 50);

            ICumulativeItem itemToJoin = new CumulativeItem(type, new Location(100, 100, 7), 100);
            sup.TryJoin(ref itemToJoin);

            Assert.Equal(50, itemToJoin.Amount);
            Assert.Equal(100, sup.Amount);
        }
        [Fact]
        public void TryJoin_When_Both_Has_Amount_100()
        {
            var type = new ItemType();
            type.SetClientId(100);

            var sup = new CumulativeItem(type, new Location(100, 100, 7), 100);

            ICumulativeItem itemToJoin = new CumulativeItem(type, new Location(100, 100, 7), 100);
            sup.TryJoin(ref itemToJoin);

            Assert.Equal(100, itemToJoin.Amount);
            Assert.Equal(100, sup.Amount);
        }

    }
}
