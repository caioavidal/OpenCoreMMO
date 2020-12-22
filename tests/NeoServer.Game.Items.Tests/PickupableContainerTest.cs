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

            container.AddThing(ItemTestData.CreateBodyEquipmentItem(100, "", "shield"));
            container.AddThing(ItemTestData.CreateBodyEquipmentItem(101, "", "shield"));

            Assert.Equal(100, container.Weight);
        }

        [Fact]
        public void Weight_When_Remove_Item_Returns_Decreased_Weight()
        {
            var container = ItemTestData.CreatePickupableContainer();
            Assert.Equal(20, container.Weight);

            container.AddThing(ItemTestData.CreateBodyEquipmentItem(100, "", "shield"));
            Assert.Equal(60, container.Weight);

            container.RemoveThing(null,1,0, out var removed);

            Assert.Equal(20, container.Weight);
        }

        [Fact]
        public void Weight_When_Add_Cumulative_Item_Returns_Increased_Weight()
        {
            var container = ItemTestData.CreatePickupableContainer();
            Assert.Equal(20, container.Weight);

            container.AddThing(ItemTestData.CreateCumulativeItem(100, 30));
            Assert.Equal(50, container.Weight);

            container.AddThing(ItemTestData.CreateCumulativeItem(100, 10));
            Assert.Equal(60, container.Weight);

            container.AddThing(ItemTestData.CreateCumulativeItem(100, 70));
            Assert.Equal(130, container.Weight);

            container.AddThing(ItemTestData.CreateCumulativeItem(105, 70));
            Assert.Equal(200, container.Weight);
        }
        [Fact]
        public void Weight_When_Remove_Cumulative_Item_Returns_Increased_Weight()
        {
            var container = ItemTestData.CreatePickupableContainer();
            Assert.Equal(20, container.Weight);

            container.AddThing(ItemTestData.CreateCumulativeItem(100, 30));
            Assert.Equal(50, container.Weight);

            container.AddThing(ItemTestData.CreateCumulativeItem(100, 10));
            Assert.Equal(60, container.Weight);
        }

        [Fact]
        public void Weight_When_Add_Item_To_Child_Container_Returns_Increased_Weight()
        {
            var sut = ItemTestData.CreatePickupableContainer();
            var child = ItemTestData.CreatePickupableContainer();

            sut.AddThing(child);

            child.AddThing(ItemTestData.CreateBodyEquipmentItem(100, "", "shield"));

            Assert.Equal(80, sut.Weight);
        }

        [Fact]
        public void Weight_When_Remove_Child_Container_Returns_Decreased_Weight()
        {
            var sut = ItemTestData.CreatePickupableContainer();
            var child = ItemTestData.CreatePickupableContainer();

            sut.AddThing(child);

            var shield = ItemTestData.CreateBodyEquipmentItem(100, "", "shield");
            child.AddThing(shield);

            Assert.Equal(80, sut.Weight);

            child.RemoveThing(null, 1, 0, out var removed);

            Assert.Equal(40, sut.Weight);

            sut.RemoveThing(null, 1, 0, out var removed2);
            Assert.Equal(20, sut.Weight);
        }

        [Fact]
        public void Weight_When_Remove_Child_Container_And_Add_Item_Returns_Same_Weight()
        {
            var sut = ItemTestData.CreatePickupableContainer();
            var child = ItemTestData.CreatePickupableContainer();

            sut.AddThing(child);

            var shield = ItemTestData.CreateBodyEquipmentItem(100, "", "shield");
            child.AddThing(shield);

            sut.RemoveThing(null, 1, 0, out var removed);
            Assert.Equal(20, sut.Weight);

            child.AddThing(ItemTestData.CreateBodyEquipmentItem(102, "", "shield"));

            Assert.Equal(20, sut.Weight);
        }

        [Fact]
        public void Weight_When_Move_Child_Container_And_Add_Item_Returns_Increased_Weight()
        {
            var sut = ItemTestData.CreatePickupableContainer();
            var child = ItemTestData.CreatePickupableContainer();
            var child2 = ItemTestData.CreatePickupableContainer();

            sut.AddThing(child);
            sut.AddThing(child2);

            var shield = ItemTestData.CreateBodyEquipmentItem(100, "", "shield");
            child.AddThing(shield);

            
            sut.SendTo(sut, child,1, 1, 0);

            Assert.Equal(100, sut.Weight);

            child.AddThing(ItemTestData.CreateBodyEquipmentItem(102, "", "shield"));

            Assert.Equal(140, sut.Weight);

            child2.AddThing(ItemTestData.CreateBodyEquipmentItem(104, "", "shield"));

            Assert.Equal(180, sut.Weight);

            child2.RemoveThing(null,1,1, out var removed);

            Assert.Equal(80, sut.Weight);
        }
    }
}
