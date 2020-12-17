using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Creature.Model;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Items.Items;
using NeoServer.Game.World.Map;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using Xunit;
using NeoServer.Game.Creatures;
using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Items.Tests
{
    public class ContainerTest
    {

        private Container CreateContainer(byte capacity = 6, string name = "")
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(Common.ItemAttribute.Capacity, capacity);
            itemType.SetName(name);
            return new PickupableContainer(itemType, new Location(100, 100, 7));
        }

        private ICumulative CreateCumulativeItem(ushort id, byte amount)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");

            return new Cumulative(type, new Location(100, 100, 7), amount);
        }

        private Item CreateRegularItem(ushort id, string name = "item")
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName(name);
            type.SetFlag(ItemFlag.Pickupable);

            return new Item(type, new Location(100, 100, 7));
        }

        [Fact]
        public void Constructor_Should_Create_Instance_With_Capacity_And_List_Items()
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(Common.ItemAttribute.Capacity, 20);

            var sut = new Container(itemType, new Location(100, 100, 7));

            Assert.Equal(20, sut.Capacity);
            Assert.NotNull(sut.Items);
            Assert.Empty(sut.Items);
        }

        [Fact]
        public void SetParent_Should_Modify_Parent_Property()
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(Common.ItemAttribute.Capacity, 20);

            var parentContainer = new Container(itemType, new Location(100, 100, 7));
            var sut = new Container(itemType, new Location(100, 100, 7));

            sut.SetParent(parentContainer);
            Assert.Equal(parentContainer, sut.Parent);
            Assert.True(sut.HasParent);

            sut.SetParent(null);
            Assert.Null(sut.Parent);
            Assert.False(sut.HasParent);
        }

        [Fact]
        public void IsApplicable_Returns_True_When_ItemType_Is_Container()
        {
            ItemType type = new ItemType();
            type.Attributes.SetAttribute(Common.ItemAttribute.Type, "container");

            Assert.True(Container.IsApplicable(type));

            type = new ItemType();
            type.SetGroup((byte)Common.ItemGroup.GroundContainer);

            Assert.True(Container.IsApplicable(type));

            type = new ItemType();
            type.Attributes.SetAttribute(Common.ItemAttribute.Type, "container");
            type.SetGroup((byte)Common.ItemGroup.GroundContainer);

            Assert.True(Container.IsApplicable(type));

            type = new ItemType();
            Assert.False(Container.IsApplicable(type));
        }

        [Fact]
        public void Get_By_Index_Should_Return_Item()
        {
            var sut = CreateContainer();
            var item1 = CreateRegularItem(100);
            var item2 = CreateRegularItem(200);
            var item3 = CreateRegularItem(300);
            var item4 = CreateRegularItem(400);

            sut.TryAddItem(item1);
            sut.TryAddItem(item2);
            sut.TryAddItem(item3);
            sut.TryAddItem(item4);

            Assert.Same(item2, sut[2]);
            Assert.Same(item1, sut[3]);
            Assert.Same(item4, sut[0]);
            Assert.Same(item3, sut[1]);
        }

        [Fact]
        public void TryAddItem_Adding_Container_Should_Add_Container_As_Child()
        {
            var sut = CreateContainer();
            var container = CreateContainer();
            sut.TryAddItem(container);

            Assert.Same(container, sut[0]);
            Assert.True(container.HasParent);
            Assert.Same(sut, container.Parent);
        }
        [Fact]
        public void TryAddItem_Adding_Container_To_Itseft_Should_Return_Error()
        {
            var sut = CreateContainer();
            var item = sut;
            var result = sut.TryAddItem(item);

            Assert.False(result.IsSuccess);
            Assert.Equal(InvalidOperation.Impossible, result.Error);
        }

        [Fact]
        public void TryAddItem_Adding_Item_To_Container_Slot_Inserts_Item_On_Target_Container()
        {
            var sut = CreateContainer();
            var container = CreateContainer();
            sut.TryAddItem(container);

            var item = CreateRegularItem(100);
            sut.TryAddItem(item, 0);

            var container2 = CreateContainer();
            sut.TryAddItem(container2, 0);

            Assert.Equal(1, sut.SlotsUsed);
            Assert.Same(item, container[1]);
            Assert.Same(container2, container[0]);
            Assert.Same(container, container2.Parent);
        }

        [Fact]
        public void TryAddItem_Adding_Item_Should_Return_False_When_Full_Capacity()
        {
            var sut = CreateContainer(3);
            sut.TryAddItem(CreateRegularItem(100));

            Assert.True(sut.TryAddItem(CreateRegularItem(100)).IsSuccess);
            Assert.True(sut.TryAddItem(CreateContainer()).IsSuccess);

            //adding items to child container
            Assert.True(sut.TryAddItem(CreateRegularItem(100), 0).IsSuccess);
            Assert.True(sut.TryAddItem(CreateRegularItem(100), 0).IsSuccess);
            Assert.True(sut.TryAddItem(CreateRegularItem(100), 0).IsSuccess);

            Assert.False(sut.TryAddItem(CreateRegularItem(100), 1).IsSuccess);
            Assert.False(sut.TryAddItem(CreateRegularItem(100), 2).IsSuccess);

            var result = sut.TryAddItem(CreateRegularItem(100), 2);
            Assert.False(result.IsSuccess);

            Assert.True(sut.TryAddItem(CreateRegularItem(100), 0).IsSuccess);

            Assert.Equal(InvalidOperation.TooHeavy, result.Error);
            Assert.Equal(3, sut.SlotsUsed);
        }

        [Fact]
        public void TryAddItem_Adding_Item_To_Child_Container_Should_Return_False_When_Full_Capacity()
        {
            var sut = CreateContainer(3);
            sut.TryAddItem(CreateRegularItem(100));
            sut.TryAddItem(CreateRegularItem(100));

            var container = CreateContainer(2);
            sut.TryAddItem(container);

            Assert.True(sut.TryAddItem(CreateContainer(100), 0).IsSuccess); //inserting on child container
            Assert.True(sut.TryAddItem(CreateContainer(100), 0).IsSuccess); //inserting on child container

            //at this point, child container is full

            Assert.False(sut.TryAddItem(CreateRegularItem(100), 0).IsSuccess); //inserting on child container
        }

        [Fact]
        public void TryAddItem_Adding_CumulativeItem_Should_Insert_Item_In_Front()
        {
            var sut = CreateContainer(5);
            sut.TryAddItem(CreateRegularItem(500));

            var item = CreateCumulativeItem(100, 40);
            sut.TryAddItem(item);

            Assert.Equal(item, sut[0]);
            Assert.Equal(40, (sut[0] as ICumulative).Amount);
            Assert.Equal(100, (sut[0] as ICumulative).ClientId);

            //adding another item

            var item2 = CreateCumulativeItem(200, 60);
            sut.TryAddItem(item2);

            Assert.Equal(item2, sut[0]);
            Assert.Equal(60, (sut[0] as ICumulative).Amount);

            //adding a regular item
            var item3 = CreateRegularItem(567);
            sut.TryAddItem(item3);

            Assert.Equal(item3, sut[0]);
            Assert.Equal(item2, sut[1]);
            Assert.Equal(item, sut[2]);

            Assert.Equal(4, sut.SlotsUsed);
        }

        [Fact]
        public void TryAddItem_Adding_Same_CumulativeItem_Should_Join()
        {
            var sut = CreateContainer(5);
            sut.TryAddItem(CreateRegularItem(500));

            var item = CreateCumulativeItem(100, 40);
            sut.TryAddItem(item);

            Assert.Equal(item, sut[0]);
            Assert.Equal(40, (sut[0] as ICumulative).Amount);
            Assert.Equal(100, (sut[0] as ICumulative).ClientId);

            //adding same item again

            var sameItemType = CreateCumulativeItem(100, 40);
            sut.TryAddItem(sameItemType);

            Assert.Equal(item, sut[0]);
            Assert.Equal(80, (sut[0] as ICumulative).Amount);

            //adding same item again. This time will exceed the amount of 100

            var sameItemType2 = CreateCumulativeItem(100, 40); //total will be 120
            sut.TryAddItem(sameItemType2);

            Assert.Equal(sameItemType2, sut[0]);
            Assert.Equal(20, (sut[0] as ICumulative).Amount);

            //adding same item again. must add to the item with amount of 20

            var sameItemType3 = CreateCumulativeItem(100, 40); //total will be 60
            sut.TryAddItem(sameItemType3);

            Assert.Equal(sameItemType2, sut[0]);
            Assert.Equal(60, (sut[0] as ICumulative).Amount);
        }

        [Fact]
        public void TryAddItem_Adding_CumulativeItem_Join_To_The_First_Less_Than_100_Item_Amount()
        {
            var sut = CreateContainer(8);
            sut.TryAddItem(CreateCumulativeItem(200, 100));
            sut.TryAddItem(CreateCumulativeItem(200, 100));
            sut.TryAddItem(CreateCumulativeItem(200, 100));
            sut.TryAddItem(CreateCumulativeItem(200, 100));

            sut.RemoveItem(1, 60);
            sut.RemoveItem(3, 77);

            Assert.Equal(100, (sut[0] as ICumulative).Amount);
            Assert.Equal(40, (sut[1] as ICumulative).Amount);
            Assert.Equal(100, (sut[2] as ICumulative).Amount);
            Assert.Equal(23, (sut[3] as ICumulative).Amount);

            var item = CreateCumulativeItem(200, 100);
            sut.TryAddItem(item);

            Assert.Equal(40, (sut[0] as ICumulative).Amount);
            Assert.Equal(100, (sut[1] as ICumulative).Amount);
            Assert.Equal(100, (sut[2] as ICumulative).Amount);
            Assert.Equal(100, (sut[3] as ICumulative).Amount);
            Assert.Equal(23, (sut[4] as ICumulative).Amount);

            sut.TryAddItem(CreateCumulativeItem(200, 60));
            sut.TryAddItem(CreateCumulativeItem(200, 10));
            Assert.Equal(100, (sut[0] as ICumulative).Amount);
            Assert.Equal(33, (sut[4] as ICumulative).Amount);
        }

        [Fact]
        public void TryAddItem_Adding_Different_CumulativeItem_Do_Not_Join()
        {
            var sut = CreateContainer(5);
            sut.TryAddItem(CreateRegularItem(500));

            var item = CreateCumulativeItem(100, 40);
            sut.TryAddItem(item);

            var item2 = CreateCumulativeItem(200, 26);
            sut.TryAddItem(item2);

            Assert.Equal(item, sut[1]);
            Assert.Equal(40, (sut[1] as ICumulative).Amount);
            Assert.Equal(100, sut[1].ClientId);

            Assert.Equal(item2, sut[0]);
            Assert.Equal(26, (sut[0] as ICumulative).Amount);
            Assert.Equal(200, sut[0].ClientId);
        }
        [Fact]
        public void TryAddItem_Adding_CumulativeItem_Return_False_When_Full()
        {
            var sut = CreateContainer(2);

            Assert.True(sut.TryAddItem(CreateCumulativeItem(100, 40)).IsSuccess);
            Assert.True(sut.TryAddItem(CreateCumulativeItem(100, 70)).IsSuccess);
            Assert.True(sut.TryAddItem(CreateCumulativeItem(100, 90)).IsSuccess);

            var result = sut.TryAddItem(CreateCumulativeItem(100, 90), 0);

            Assert.False(result.IsSuccess);
            Assert.Equal(InvalidOperation.IsFull, result.Error);

            result = sut.TryAddItem(CreateCumulativeItem(200, 90));
            Assert.False(result.IsSuccess);
            Assert.Equal(InvalidOperation.IsFull, result.Error);
        }
        [Fact]
        public void MoveItem_When_Moving_Cumulative_To_Child_Should_Remove_From_Parent_And_Add_To_Child()
        {
            var sut = CreateContainer(2);
            var child = CreateContainer(1);
            var item = CreateCumulativeItem(100, 40);

            sut.TryAddItem(child);
            sut.TryAddItem(item);

            var eventCalled = false;
            var childEventCalled = false;

            sut.OnItemRemoved += (a, b) => { eventCalled = true; };
            child.OnItemAdded += (a) => { childEventCalled = true; };

            sut.MoveItem((byte)item.Location.ContainerSlot, (byte)child.Location.ContainerSlot, 40);

            Assert.True(eventCalled);
            Assert.True(childEventCalled);

            Assert.Single(sut.Items);
            Assert.Equal(40, (child[0] as ICumulative).Amount);
        }
        [Fact]
        public void MoveItem_When_Moving_To_Child_Should_Remove_From_Parent_And_Add_To_Child()
        {
            var sut = CreateContainer(2);
            var child = CreateContainer(1);
            var item = ItemTestData.CreateBodyEquipmentItem(100, "head");

            sut.TryAddItem(child);
            sut.TryAddItem(item);

            var eventCalled = false;
            var childEventCalled = false;

            sut.OnItemRemoved += (a, b) => { eventCalled = true; };
            child.OnItemAdded += (a) => { childEventCalled = true; };

            sut.MoveItem((byte)item.Location.ContainerSlot, (byte)child.Location.ContainerSlot);

            Assert.True(eventCalled);
            Assert.True(childEventCalled);

            Assert.Single(sut.Items);
            Assert.Equal(100, child[0].ClientId);
        }
        [Fact]
        public void MoveItem_When_Moving_Cumulative_To_Child_And_Is_Full_Return_Error()
        {
            var sut = CreateContainer(2);
            var child = CreateContainer(1);
            var item = CreateCumulativeItem(100, 40);
            var item2 = CreateCumulativeItem(100, 100);

            sut.TryAddItem(child);
            sut.TryAddItem(item);
            child.TryAddItem(item2);

            var result = sut.MoveItem((byte)item.Location.ContainerSlot, (byte)child.Location.ContainerSlot, 40);

            Assert.Equal(InvalidOperation.IsFull, result.Error);
            Assert.Equal(2, sut.Items.Count);
            Assert.Equal(100, (child[0] as ICumulative).Amount);
        }
        [Fact]
        public void MoveItem_When_Moving_Cumulative_To_Child_Should_Remove_Amount_From_Parent_And_Add_To_Child()
        {
            var sut = CreateContainer(2);
            var child = CreateContainer(1);
            var item = CreateCumulativeItem(100, 40);
            var item2 = CreateCumulativeItem(100, 20);

            sut.TryAddItem(child);
            sut.TryAddItem(item);
            child.TryAddItem(item2);

            var eventCalled = false;
            var childEventCalled = false;

            sut.OnItemUpdated += (a, b, c) => { eventCalled = true; };
            child.OnItemUpdated += (a, b, c) => { childEventCalled = true; };

            var result = sut.MoveItem((byte)item.Location.ContainerSlot, (byte)child.Location.ContainerSlot, 20);

            Assert.True(eventCalled);
            Assert.True(childEventCalled);

            Assert.Equal(2, sut.Items.Count);
            Assert.Equal(20, (sut[(byte)item.Location.ContainerSlot] as ICumulative).Amount);
            Assert.Equal(40, (child[(byte)item2.Location.ContainerSlot] as ICumulative).Amount);
        }
        [Fact]
        public void TryAddItem_Adding_CumulativeItem_Rejects_Exceeding_Amount_When_Full()
        {
            var sut = CreateContainer(2);

            Assert.True(sut.TryAddItem(CreateCumulativeItem(100, 70)).IsSuccess);
            Assert.True(sut.TryAddItem(CreateCumulativeItem(100, 70)).IsSuccess);

            var result = sut.TryAddItem(CreateCumulativeItem(100, 90), 0);
            Assert.False(result.IsSuccess);

            Assert.Equal(InvalidOperation.TooHeavy, result.Error);

            Assert.Equal(100, (sut[0] as ICumulative).Amount);
            Assert.Equal(100, (sut[1] as ICumulative).Amount);

            Assert.True(sut.IsFull);
        }

        [Fact]
        public void RemoveItem_RegularItem_Removes_From_Container()
        {
            var sut = CreateContainer(2);

            var item = CreateRegularItem(100);
            sut.TryAddItem(item);

            var item2 = CreateRegularItem(100);
            sut.TryAddItem(item2);

            var removedItem = sut.RemoveItem(0);

            Assert.Equal(1, sut.SlotsUsed);
            Assert.Equal(item, sut[0]);
            Assert.Same(item2, removedItem);
        }

        [Fact]
        public void RemoveItem_When_Container_Removes_Item_And_Remove_Parent()
        {
            var sut = CreateContainer(2);

            var container = CreateContainer();
            sut.TryAddItem(container);
            sut.TryAddItem(CreateRegularItem(100));

            Assert.Same(sut, container.Parent);

            sut.RemoveItem(1);

            Assert.Equal(1, sut.SlotsUsed);
            Assert.False(container.HasParent);
        }

        [Fact]
        public void RemoveItem_When_Cumumulative_Removes_Amount()
        {
            var sut = CreateContainer(2);
            sut.TryAddItem(CreateRegularItem(100));

            var item = CreateCumulativeItem(200, 57);
            sut.TryAddItem(item);

            var removedItem = sut.RemoveItem(0, 23);

            Assert.Equal(2, sut.SlotsUsed);

            Assert.Equal(item, sut[0]);

            Assert.Equal(34, (sut[0] as ICumulative).Amount);
            Assert.Equal(23, (removedItem as ICumulative).Amount);

            Assert.NotSame(item, removedItem);
            Assert.Equal(item.ClientId, removedItem.ClientId);
        }

        [Fact]
        public void RemoveItem_When_Remove_Full_Amount_Removes_Item()
        {
            var sut = CreateContainer(2);
            sut.TryAddItem(CreateRegularItem(100));

            var item = CreateCumulativeItem(200, 63);
            sut.TryAddItem(item);

            var removedItem = sut.RemoveItem(0, 63);

            Assert.Equal(1, sut.SlotsUsed);

            Assert.Equal(100, (sut[0].ClientId));
            Assert.Equal(63, (removedItem as ICumulative).Amount);

            Assert.NotSame(item, removedItem);
            Assert.Equal(item.ClientId, removedItem.ClientId);
        }

        [Fact]
        public void ToString_When_Container_Is_Empty_Should_Return_Nothing()
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(Common.ItemAttribute.Capacity, 20);

            var sut = new Container(itemType, new Location(100, 100, 7));

            Assert.Equal("nothing", sut.ToString());
        }
        [Fact]
        public void ToString_When_Container_Has_Items_Should_Return_Items_Name()
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(Common.ItemAttribute.Capacity, 20);

            var sut = new Container(itemType, new Location(100, 100, 7));

            var item = CreateRegularItem(100, "item 1");
            sut.TryAddItem(item);

            var item2 = CreateRegularItem(100, "item 2");
            sut.TryAddItem(item2);

            var item3 = CreateRegularItem(100, "item 3");

            var container = CreateContainer(20, "bag");
            container.TryAddItem(item3);

            sut.TryAddItem(container);

            Assert.Equal("bag, item 3, item 2, item 1", sut.ToString());
        }

        [Fact]
        public void TryAddItem_Changes_Item_Location()
        {
            var sut = CreateContainer(5);
            var item = ItemTestData.CreateWeaponItem(100, "axe");
            sut.TryAddItem(item);

            Assert.Equal(Location.Container(0, 0), item.Location);
        }

    }
}
