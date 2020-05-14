using NeoServer.Game.Items.Items;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NeoServer.Game.Items.Tests
{
    public class PickupableContainerTest
    {
        [Fact]
        public void Weight_When_Add_Item_Returns_Increased_Weight()
        {
            var container = ItemTestData.CreatePickupableContainer();
            Assert.Equal(20, container.Weight);

            container.TryAddItem(ItemTestData.CreateBodyEquipmentItem(100, "", "shield"));
            container.TryAddItem(ItemTestData.CreateBodyEquipmentItem(101, "", "shield"));

            Assert.Equal(100, container.Weight);
        }

        [Fact]
        public void Weight_When_Add_Item_Returns_Decreased_Weight()
        {
            var container = ItemTestData.CreatePickupableContainer();
            Assert.Equal(20, container.Weight);

            container.TryAddItem(ItemTestData.CreateBodyEquipmentItem(100, "", "shield"));
            Assert.Equal(60, container.Weight);

            container.RemoveItem(0);

            Assert.Equal(20, container.Weight);
        }
    }
}
