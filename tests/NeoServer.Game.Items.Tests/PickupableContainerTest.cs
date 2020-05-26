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
        public void Weight_When_Remove_Item_Returns_Decreased_Weight()
        {
            var container = ItemTestData.CreatePickupableContainer();
            Assert.Equal(20, container.Weight);

            container.TryAddItem(ItemTestData.CreateBodyEquipmentItem(100, "", "shield"));
            Assert.Equal(60, container.Weight);

            container.RemoveItem(0);

            Assert.Equal(20, container.Weight);
        }

        [Fact]
        public void Weight_When_Add_Cumulative_Item_Returns_Increased_Weight()
        {
            var container = ItemTestData.CreatePickupableContainer();
            Assert.Equal(20, container.Weight);

            container.TryAddItem(ItemTestData.CreateCumulativeItem(100, 30));
            Assert.Equal(50, container.Weight);

            container.TryAddItem(ItemTestData.CreateCumulativeItem(100, 10));
            Assert.Equal(60, container.Weight);

            container.TryAddItem(ItemTestData.CreateCumulativeItem(100, 70));
            Assert.Equal(130, container.Weight);

            container.TryAddItem(ItemTestData.CreateCumulativeItem(105, 70));
            Assert.Equal(200, container.Weight);
        }
        [Fact]
        public void Weight_When_Remove_Cumulative_Item_Returns_Increased_Weight()
        {
            var container = ItemTestData.CreatePickupableContainer();
            Assert.Equal(20, container.Weight);

            container.TryAddItem(ItemTestData.CreateCumulativeItem(100, 30));
            Assert.Equal(50, container.Weight);

            container.TryAddItem(ItemTestData.CreateCumulativeItem(100, 10));
            Assert.Equal(60, container.Weight);
        }

        [Fact]
        public void Weight_When_Add_Item_To_Child_Container_Returns_Increased_Weight()
        {
            var sut = ItemTestData.CreatePickupableContainer();
            var child = ItemTestData.CreatePickupableContainer();

            sut.TryAddItem(child);

            child.TryAddItem(ItemTestData.CreateBodyEquipmentItem(100, "", "shield"));

            Assert.Equal(80, sut.Weight);
        }

        [Fact]
        public void Weight_When_Remove_Child_Container_Returns_Decreased_Weight()
        {
            var sut = ItemTestData.CreatePickupableContainer();
            var child = ItemTestData.CreatePickupableContainer();

            sut.TryAddItem(child);

            var shield = ItemTestData.CreateBodyEquipmentItem(100, "", "shield");
            child.TryAddItem(shield);

            Assert.Equal(80, sut.Weight);

            child.RemoveItem(0);

            Assert.Equal(40, sut.Weight);

            sut.RemoveItem(0);
            Assert.Equal(20, sut.Weight);
        }

        [Fact]
        public void Weight_When_Remove_Child_Container_And_Add_Item_Returns_Same_Weight()
        {
            var sut = ItemTestData.CreatePickupableContainer();
            var child = ItemTestData.CreatePickupableContainer();

            sut.TryAddItem(child);

            var shield = ItemTestData.CreateBodyEquipmentItem(100, "", "shield");
            child.TryAddItem(shield);

            sut.RemoveItem(0);
            Assert.Equal(20, sut.Weight);

            child.TryAddItem(ItemTestData.CreateBodyEquipmentItem(102, "", "shield"));

            Assert.Equal(20, sut.Weight);
        }

        [Fact]
        public void Weight_When_Move_Child_Container_And_Add_Item_Returns_Increased_Weight()
        {
            var sut = ItemTestData.CreatePickupableContainer();
            var child = ItemTestData.CreatePickupableContainer();
            var child2 = ItemTestData.CreatePickupableContainer();

            sut.TryAddItem(child);
            sut.TryAddItem(child2);

            var shield = ItemTestData.CreateBodyEquipmentItem(100, "", "shield");
            child.TryAddItem(shield);

            sut.MoveItem(1, 0);

            Assert.Equal(100, sut.Weight);

            child.TryAddItem(ItemTestData.CreateBodyEquipmentItem(102, "", "shield"));

            Assert.Equal(140, sut.Weight);

            child2.TryAddItem(ItemTestData.CreateBodyEquipmentItem(104, "", "shield"));

            Assert.Equal(180, sut.Weight);

            child2.RemoveItem(1);

            Assert.Equal(80, sut.Weight);
        }
    }
}
