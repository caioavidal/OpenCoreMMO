using System.Collections.Generic;
using FluentAssertions;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Monster.Loot;
using NeoServer.Game.Items.Bases;
using NeoServer.Game.Items.Items.Containers;
using NeoServer.Game.Items.Items.Cumulatives;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.Tests.Helpers.Player;
using Xunit;

namespace NeoServer.Game.Items.Tests;

public class ContainerTests
{
    private IContainer CreateContainer(byte capacity = 6, string name = "")
    {
        var itemType = new ItemType();
        itemType.Attributes.SetAttribute(ItemAttribute.Capacity, capacity);
        itemType.SetName(name);
        return new PickupableContainer(itemType, new Location(100, 100, 7), null);
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
        itemType.Attributes.SetAttribute(ItemAttribute.Capacity, 20);

        var sut = new Container(itemType, new Location(100, 100, 7));

        Assert.Equal(20, sut.Capacity);
        Assert.NotNull(sut.Items);
        Assert.Empty(sut.Items);
    }

    [Fact]
    public void SetParent_Should_Modify_Parent_Property()
    {
        var itemType = new ItemType();
        itemType.Attributes.SetAttribute(ItemAttribute.Capacity, 20);

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
        var type = new ItemType();
        type.Attributes.SetAttribute(ItemAttribute.Type, "container");

        Assert.True(Container.IsApplicable(type));

        type = new ItemType();
        type.SetGroup((byte)ItemGroup.GroundContainer);

        Assert.True(Container.IsApplicable(type));

        type = new ItemType();
        type.Attributes.SetAttribute(ItemAttribute.Type, "container");
        type.SetGroup((byte)ItemGroup.GroundContainer);

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

        sut.AddItem(item1);
        sut.AddItem(item2);
        sut.AddItem(item3);
        sut.AddItem(item4);

        Assert.Same(item2, sut[2]);
        Assert.Same(item1, sut[3]);
        Assert.Same(item4, sut[0]);
        Assert.Same(item3, sut[1]);
    }

    [Fact]
    public void PossibleAmountToAdd_When_Passing_Regular_Item_Should_Return_Empty_Slots_Count()
    {
        var sut = CreateContainer(1);
        var item = ItemTestData.CreateRegularItem(100);

        var result = sut.PossibleAmountToAdd(item);
        Assert.Equal(1u, result);

        sut.AddItem(item);
        result = sut.PossibleAmountToAdd(item);
        Assert.Equal(0u, result);
    }

    [Fact]
    public void PossibleAmountToAdd_When_Passing_Cumulative_Item_Should_Return_Amount_Count()
    {
        var sut = CreateContainer(3);
        var item = ItemTestData.CreateAmmo(100, 100);

        var result = sut.PossibleAmountToAdd(item);
        Assert.Equal(300u, result);

        sut.AddItem(item);

        result = sut.PossibleAmountToAdd(item);
        Assert.Equal(200u, result);

        var item2 = ItemTestData.CreateAmmo(100, 30);

        sut.AddItem(item2);
        result = sut.PossibleAmountToAdd(item);
        Assert.Equal(170u, result);

        var item3 = ItemTestData.CreateAmmo(200, 100);

        sut.AddItem(item3);
        result = sut.PossibleAmountToAdd(item);
        Assert.Equal(70u, result);

        var item4 = ItemTestData.CreateAmmo(100, 70);

        sut.AddItem(item4);
        result = sut.PossibleAmountToAdd(item);
        Assert.Equal(0u, result);
    }

    [Fact]
    public void TryAddItem_Adding_Container_Should_Add_Container_As_Child()
    {
        var sut = CreateContainer();
        var container = CreateContainer();
        sut.AddItem(container);

        Assert.Same(container, sut[0]);
        Assert.True(container.HasParent);
        Assert.Same(sut, container.Parent);
    }

    [Fact]
    public void TryAddItem_Adding_Container_To_Itseft_Should_Return_Error()
    {
        var sut = CreateContainer();
        var item = sut;
        var result = sut.AddItem(item);

        Assert.False(result.Succeeded);
        Assert.Equal(InvalidOperation.Impossible, result.Error);
    }

    [Fact]
    public void TryAddItem_Adding_Item_To_Container_Slot_Inserts_Item_On_Target_Container()
    {
        var sut = CreateContainer();
        var container = CreateContainer();
        sut.AddItem(container);

        var item = CreateRegularItem(100);
        sut.AddItem(item, 0);

        var container2 = CreateContainer();
        sut.AddItem(container2, 0);

        Assert.Equal(1, sut.SlotsUsed);
        Assert.Same(item, container[1]);
        Assert.Same(container2, container[0]);
        Assert.Same(container, container2.Parent);
    }

    [Fact]
    public void TryAddItem_Adding_Item_Should_Return_False_When_Full_Capacity()
    {
        var sut = CreateContainer(3);
        sut.AddItem(CreateRegularItem(100));

        Assert.True(sut.AddItem(CreateRegularItem(100)).Succeeded);
        Assert.True(sut.AddItem(CreateContainer()).Succeeded);

        //adding items to child container
        Assert.True(sut.AddItem(CreateRegularItem(100), 0).Succeeded);
        Assert.True(sut.AddItem(CreateRegularItem(100), 0).Succeeded);
        Assert.True(sut.AddItem(CreateRegularItem(100), 0).Succeeded);

        Assert.False(sut.AddItem(CreateRegularItem(100), 1).Succeeded);
        Assert.False(sut.AddItem(CreateRegularItem(100), 2).Succeeded);

        var result = sut.AddItem(CreateRegularItem(100), 2);
        Assert.False(result.Succeeded);

        Assert.True(sut.AddItem(CreateRegularItem(100), 0).Succeeded);

        Assert.Equal(InvalidOperation.IsFull, result.Error);
        Assert.Equal(3, sut.SlotsUsed);
    }

    [Fact]
    public void TryAddItem_Adding_Item_To_Child_Container_Should_Return_False_When_Full_Capacity()
    {
        var sut = CreateContainer(3);
        sut.AddItem(CreateRegularItem(100));
        sut.AddItem(CreateRegularItem(100));

        var container = CreateContainer(2);
        sut.AddItem(container);

        Assert.True(sut.AddItem(CreateContainer(100), 0).Succeeded); //inserting on child container
        Assert.True(sut.AddItem(CreateContainer(100), 0).Succeeded); //inserting on child container

        //at this point, child container is full

        Assert.False(sut.AddItem(CreateRegularItem(100), 0).Succeeded); //inserting on child container
    }

    [Fact]
    public void TryAddItem_Adding_CumulativeItem_Should_Insert_Item_In_Front()
    {
        var sut = CreateContainer(5);
        sut.AddItem(CreateRegularItem(500));

        var item = CreateCumulativeItem(100, 40);
        sut.AddItem(item);

        Assert.Equal(item, sut[0]);
        Assert.Equal(40, (sut[0] as ICumulative).Amount);
        Assert.Equal(100, (sut[0] as ICumulative).ClientId);

        //adding another item

        var item2 = CreateCumulativeItem(200, 60);
        sut.AddItem(item2);

        Assert.Equal(item2, sut[0]);
        Assert.Equal(60, (sut[0] as ICumulative).Amount);

        //adding a regular item
        var item3 = CreateRegularItem(567);
        sut.AddItem(item3);

        Assert.Equal(item3, sut[0]);
        Assert.Equal(item2, sut[1]);
        Assert.Equal(item, sut[2]);

        Assert.Equal(4, sut.SlotsUsed);
    }

    [Fact]
    public void TryAddItem_Adding_Same_CumulativeItem_Should_Join()
    {
        var sut = CreateContainer(5);
        sut.AddItem(CreateRegularItem(500));

        var item = CreateCumulativeItem(100, 40);
        sut.AddItem(item);

        Assert.Equal(item, sut[0]);
        Assert.Equal(40, (sut[0] as ICumulative).Amount);
        Assert.Equal(100, (sut[0] as ICumulative).ClientId);

        //adding same item again

        var sameItemType = CreateCumulativeItem(100, 40);
        sut.AddItem(sameItemType);

        Assert.Equal(item, sut[0]);
        Assert.Equal(80, (sut[0] as ICumulative).Amount);

        //adding same item again. This time will exceed the amount of 100

        var sameItemType2 = CreateCumulativeItem(100, 40); //total will be 120
        sut.AddItem(sameItemType2);

        Assert.Equal(sameItemType2, sut[0]);
        Assert.Equal(20, (sut[0] as ICumulative).Amount);

        //adding same item again. must add to the item with amount of 20

        var sameItemType3 = CreateCumulativeItem(100, 40); //total will be 60
        sut.AddItem(sameItemType3);

        Assert.Equal(sameItemType2, sut[0]);
        Assert.Equal(60, (sut[0] as ICumulative).Amount);
    }

    [Fact]
    public void TryAddItem_Adding_CumulativeItem_Join_To_The_First_Less_Than_100_Item_Amount()
    {
        var sut = CreateContainer(8);
        sut.AddItem(CreateCumulativeItem(200, 100));
        sut.AddItem(CreateCumulativeItem(200, 100));
        sut.AddItem(CreateCumulativeItem(200, 100));
        sut.AddItem(CreateCumulativeItem(200, 100));

        sut.RemoveItem(null, 60, 1, out var removedThing);
        sut.RemoveItem(null, 77, 3, out var removedThing2);

        Assert.Equal(100, (sut[0] as ICumulative).Amount);
        Assert.Equal(40, (sut[1] as ICumulative).Amount);
        Assert.Equal(100, (sut[2] as ICumulative).Amount);
        Assert.Equal(23, (sut[3] as ICumulative).Amount);

        var item = CreateCumulativeItem(200, 100);
        sut.AddItem(item);

        Assert.Equal(40, (sut[0] as ICumulative).Amount);
        Assert.Equal(100, (sut[1] as ICumulative).Amount);
        Assert.Equal(100, (sut[2] as ICumulative).Amount);
        Assert.Equal(100, (sut[3] as ICumulative).Amount);
        Assert.Equal(23, (sut[4] as ICumulative).Amount);

        sut.AddItem(CreateCumulativeItem(200, 60));
        sut.AddItem(CreateCumulativeItem(200, 10));
        Assert.Equal(100, (sut[0] as ICumulative).Amount);
        Assert.Equal(33, (sut[4] as ICumulative).Amount);
    }

    [Fact]
    public void TryAddItem_Adding_Different_CumulativeItem_Do_Not_Join()
    {
        var sut = CreateContainer(5);
        sut.AddItem(CreateRegularItem(500));

        var item = CreateCumulativeItem(100, 40);
        sut.AddItem(item);

        var item2 = CreateCumulativeItem(200, 26);
        sut.AddItem(item2);

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

        Assert.True(sut.AddItem(CreateCumulativeItem(100, 40)).Succeeded);
        Assert.True(sut.AddItem(CreateCumulativeItem(100, 70)).Succeeded);
        Assert.True(sut.AddItem(CreateCumulativeItem(100, 90)).Succeeded);

        var result = sut.AddItem(CreateCumulativeItem(100, 90), 0);

        Assert.False(result.Succeeded);
        Assert.Equal(InvalidOperation.IsFull, result.Error);

        result = sut.AddItem(CreateCumulativeItem(200, 90));
        Assert.False(result.Succeeded);
        Assert.Equal(InvalidOperation.IsFull, result.Error);
    }

    [Fact]
    public void TryAddItem_Adding_CumulativeItem_Join_If_Possible_And_Return_Full_If_Exceeds()
    {
        var sut = CreateContainer(2);

        Assert.True(sut.AddItem(CreateCumulativeItem(100, 40)).Succeeded);
        Assert.True(sut.AddItem(CreateCumulativeItem(100, 70)).Succeeded);
        Assert.True(sut.AddItem(CreateCumulativeItem(100, 40)).Succeeded);
        //total amount of 150
        var item = CreateCumulativeItem(100, 70);
        var result = sut.AddItem(item); //should join only 50 and return full

        Assert.False(result.Succeeded);
        Assert.Equal(InvalidOperation.IsFull, result.Error);

        Assert.Equal(100, (sut[0] as ICumulative).Amount);
        Assert.Equal(100, (sut[1] as ICumulative).Amount);
        Assert.Equal(20, item.Amount);
    }

    [Fact]
    public void TryAddItem_Adding_CumulativeItem_Rejects_Exceeding_Amount_When_Full()
    {
        var sut = CreateContainer(2);

        Assert.True(sut.AddItem(CreateCumulativeItem(100, 70)).Succeeded);
        Assert.True(sut.AddItem(CreateCumulativeItem(100, 70)).Succeeded);

        var result = sut.AddItem(CreateCumulativeItem(100, 90), 0);
        Assert.False(result.Succeeded);

        Assert.Equal(InvalidOperation.IsFull, result.Error);

        Assert.Equal(100, ((ICumulative)sut[0]).Amount);
        Assert.Equal(100, ((ICumulative)sut[1]).Amount);

        Assert.True(sut.IsFull);
    }

    [Fact]
    public void RemoveItem_RegularItem_Removes_From_Container()
    {
        var sut = CreateContainer(2);

        var item = CreateRegularItem(100);
        sut.AddItem(item);

        var item2 = CreateRegularItem(100);
        sut.AddItem(item2);

        sut.RemoveItem(null, 1, 0, out var removedItem);

        Assert.Equal(1, sut.SlotsUsed);
        Assert.Equal(item, sut[0]);
        Assert.Same(item2, removedItem);
    }

    [Fact]
    public void RemoveItem_When_Container_Removes_Item_And_Remove_Parent()
    {
        var sut = CreateContainer(2);

        var container = CreateContainer();
        sut.AddItem(container);
        sut.AddItem(CreateRegularItem(100));

        Assert.Same(sut, container.Parent);

        sut.RemoveItem(null, 1, 1, out var removedThing);

        Assert.Equal(1, sut.SlotsUsed);
        Assert.False(container.HasParent);
    }

    [Fact]
    public void RemoveItem_When_Cumumulative_Removes_Amount()
    {
        var sut = CreateContainer(2);
        sut.AddItem(CreateRegularItem(100));

        var item = CreateCumulativeItem(200, 57);
        sut.AddItem(item);

        sut.RemoveItem(null, 23, 0, out var removedItem);

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
        sut.AddItem(CreateRegularItem(100));

        var item = CreateCumulativeItem(200, 63);
        sut.AddItem(item);

        sut.RemoveItem(null, 63, 0, out var removedItem);

        Assert.Equal(1, sut.SlotsUsed);

        Assert.Equal(100, sut[0].ClientId);
        Assert.Equal(63, (removedItem as ICumulative).Amount);

        Assert.Equal(item.ClientId, removedItem.ClientId);
    }

    [Fact]
    public void ToString_When_Container_Is_Empty_Should_Return_Nothing()
    {
        var itemType = new ItemType();
        itemType.Attributes.SetAttribute(ItemAttribute.Capacity, 20);

        var sut = new Container(itemType, new Location(100, 100, 7));

        Assert.Equal("nothing", sut.ToString());
    }

    [Fact]
    public void ToString_When_Container_Has_Items_Should_Return_Items_Name()
    {
        var itemType = new ItemType();
        itemType.Attributes.SetAttribute(ItemAttribute.Capacity, 20);

        var sut = new Container(itemType, new Location(100, 100, 7));

        var item = CreateRegularItem(100, "item 1");
        sut.AddItem(item);

        var item2 = CreateRegularItem(100, "item 2");
        sut.AddItem(item2);

        var item3 = CreateRegularItem(100, "item 3");

        var container = CreateContainer(20, "bag");
        container.AddItem(item3);

        sut.AddItem(container);

        Assert.Equal("bag, item 3, item 2, item 1", sut.ToString());
    }

    [Fact]
    public void TryAddItem_Changes_Item_Location()
    {
        var sut = CreateContainer(5);
        var item = ItemTestData.CreateWeaponItem(100, "axe");
        sut.AddItem(item);

        Assert.Equal(Location.Container(0, 0), item.Location);
    }

    [Fact]
    public void CanAddItem_Adding_Regular_Item_With_No_Free_Slots_Returns_Error()
    {
        var sut = CreateContainer(0);
        var item = ItemTestData.CreateWeaponItem(100, "axe");
        var result = sut.CanAddItem(item.Metadata);

        Assert.Equal(InvalidOperation.NotEnoughRoom, result.Error);
    }

    [Fact]
    public void CanAddItem_Adding_Cumulative_Item_With_No_Free_Slots_Returns_Error()
    {
        var sut = CreateContainer(0);
        var item = ItemTestData.CreateCumulativeItem(1, 100);
        var result = sut.CanAddItem(item.Metadata);

        Assert.Equal(InvalidOperation.NotEnoughRoom, result.Error);
    }

    [Fact]
    public void CanAddItem_Adding_Regular_Item_With_Free_Slots_Returns_Success()
    {
        var sut = CreateContainer(1);
        var item = ItemTestData.CreateWeaponItem(1, "axe");
        var result = sut.CanAddItem(item.Metadata);

        Assert.Equal(1u, result.Value);
    }

    [Fact]
    public void CanAddItem_Adding_Regular_Item_With_Free_Slots_On_Child_Returns_Success()
    {
        var sut = CreateContainer(1);
        var child = CreateContainer(1);

        sut.AddItem(child);

        var item = ItemTestData.CreateWeaponItem(1, "axe");

        var result = sut.CanAddItem(item.Metadata);

        Assert.Equal(1u, result.Value);
    }

    [Fact]
    public void CanAddItem_Adding_Cumulative_Item_With_Free_Slots_Returns_Success()
    {
        var sut = CreateContainer(1);

        var item = ItemTestData.CreateAmmo(1, 100);

        var result = sut.CanAddItem(item.Metadata);

        Assert.Equal(100u, result.Value);
    }

    [Fact]
    public void CanAddItem_Adding_Cumulative_Item_With_Partial_Free_Slots_Returns_Success()
    {
        var sut = CreateContainer(1);

        sut.AddItem(ItemTestData.CreateAmmo(1, 50));

        var item = ItemTestData.CreateAmmo(1, 100);

        var result = sut.CanAddItem(item.Metadata);

        Assert.Equal(50u, result.Value);
    }

    [Fact]
    public void CanAddItem_Adding_Cumulative_Item_With_Partial_Free_Slots_With_Diff_Type_Returns_Error()
    {
        var sut = CreateContainer(1);

        sut.AddItem(ItemTestData.CreateAmmo(2, 50));

        var item = ItemTestData.CreateAmmo(1, 100);

        var result = sut.CanAddItem(item.Metadata);

        Assert.Equal(InvalidOperation.NotEnoughRoom, result.Error);
    }

    [Fact]
    public void CanBeDressed_ReturnsTrue()
    {
        //arrange
        var player = PlayerTestDataBuilder.Build();
        var sut = CreateContainer(1);

        //act
        var actual = sut.CanBeDressed(player);
        //assert
        actual.Should().BeTrue();
    }


    [Fact]
    public void Container_has_children_items_when_created()
    {
        //arrange
        var children = new List<IItem>
        {
            ItemTestData.CreateWeaponItem(1),
            ItemTestData.CreateContainer(2),
            ItemTestData.CreateAttackRune(3, amount: 55)
        };

        //act
        var sut = ItemTestData.CreateContainer(5, children: children);

        //assert
        sut.Items[0].Should().Be(children[0]);

        sut.Items[1].Should().Be(children[1]);

        sut.Items[2].Should().Be(children[2]);
        sut.Items[2].Amount.Should().Be(55);
    }

    [Fact]
    public void ToString_shows_container_content()
    {
        //arrange

        var food = ItemTestData.CreateCumulativeItem(1, 1, "meat");
        var children = new List<IItem>
        {
            ItemTestData.CreateWeaponItem(1, name: "sabre"),
            ItemTestData.CreateContainer(2, children: new List<IItem> { food }),
            ItemTestData.CreateAttackRune(3, amount: 55)
        };

        var sut = ItemTestData.CreateContainer(5, children: children);

        //act
        var result = sut.ToString();
        //assert
        result.Should().Be("a sabre, a bag, meat, 55 hmms");
    }

    [Fact]
    public void LootContainer_toString_shows_loot_content()
    {
        //arrange

        var food = ItemTestData.CreateCumulativeItem(1, 1, "meat");

        var loot = new Loot(new ILootItem[]
        {
            new LootItem(() => ItemTestData.CreateWeaponItem(1, name: "sabre").Metadata, 1, 1, null),
            new LootItem(() => ItemTestData.CreateContainer(2).Metadata, 1, 1,
                new ILootItem[] { new LootItem(() => food.Metadata, 1, 1, null) }),
            new LootItem(() => ItemTestData.CreateAttackRune(3, amount: 55).Metadata, 55, 1, null)
        }, new HashSet<ICreature>(0));

        var sut = ItemTestData.CreateLootContainer(5, loot: loot);

        //act
        var result = sut.ToString();
        //assert
        result.Should().Be("a sabre, a bag, meat, 55 hmms");
    }

    [Fact]
    public void Bag_created_with_items_inside_weights_sum_of_all_items_weight()
    {
        //arrange
        var item1 = ItemTestData.CreateWeaponItem(id: 100);
        var item2 = ItemTestData.CreateWeaponItem(id: 101);
        var item3 = ItemTestData.CreateCumulativeItem(id: 102, amount: 10);

        var bag = ItemTestData.CreateContainer(weight: 20, children: new List<IItem> { item3 });

        var container = ItemTestData.CreateContainer(weight: 10, children: new List<IItem> { item1, item2, bag });

        //assert
        container.Weight.Should().Be(120);
    }

    #region Remove Items

    [Fact]
    public void Player_removes_item_from_container()
    {
        //arrange
        var item = ItemTestData.CreateWeaponItem(1);
        var children = new List<IItem>
        {
            item,
            ItemTestData.CreateContainer(2),
            ItemTestData.CreateAttackRune(3, amount: 55)
        };

        var sut = ItemTestData.CreateContainer(5, children: children);


        //act
        sut.RemoveItem(item.Metadata, 1);

        //assert
        sut.Items.Count.Should().Be(2);
        sut.Items.Should().NotContain(item);
    }

    [Fact]
    public void Player_removes_item_from_container_within_another_container()
    {
        //arrange
        var item = ItemTestData.CreateWeaponItem(1);
        var innerContainer = ItemTestData.CreateContainer(2);

        innerContainer.AddItem(item);
        innerContainer.AddItem(ItemTestData.CreateWeaponItem(1));

        var children = new List<IItem>
        {
            innerContainer,
            ItemTestData.CreateAttackRune(3, amount: 55)
        };

        var sut = ItemTestData.CreateContainer(5, children: children);

        //act
        sut.RemoveItem(item.Metadata, 2);

        //assert
        sut.Items.Count.Should().Be(2);
        innerContainer.Items.Should().BeEmpty();
    }

    [Fact]
    public void Player_removes_two_items_from_container_and_one_remains()
    {
        //arrange
        var item = ItemTestData.CreateWeaponItem(1);
        var item2 = ItemTestData.CreateWeaponItem(1);
        var item3 = ItemTestData.CreateWeaponItem(1);

        var children = new List<IItem>
        {
            item,
            item2,
            item3
        };

        var sut = ItemTestData.CreateContainer(5, children: children);

        //act
        sut.RemoveItem(item.Metadata, 2);

        //assert
        sut.Items.Count.Should().Be(1);
        sut.Items.Should().Contain(item3);
    }

    [Fact]
    public void Player_removes_inner_container_from_container()
    {
        //arrange
        var innerContainer = ItemTestData.CreateContainer(10);
        var item2 = ItemTestData.CreateWeaponItem(1);
        var item3 = ItemTestData.CreateWeaponItem(1);

        innerContainer.AddItem(item2);
        innerContainer.AddItem(item3);

        var children = new List<IItem>
        {
            innerContainer
        };

        var sut = ItemTestData.CreateContainer(5, children: children);

        //act
        sut.RemoveItem(innerContainer.Metadata, 10);

        //assert
        sut.Items.Count.Should().Be(0);
        sut.Items.Should().BeEmpty();
    }

    [Fact]
    public void Player_removes_all_cumulative_from_container()
    {
        //arrange
        var innerContainer = ItemTestData.CreateContainer(10);
        var cumulative = ItemTestData.CreateCumulativeItem(1, 50);
        var item3 = ItemTestData.CreateWeaponItem(1);

        innerContainer.AddItem(item3);

        var children = new List<IItem>
        {
            cumulative,
            innerContainer
        };

        var sut = ItemTestData.CreateContainer(5, children: children);

        //act
        sut.RemoveItem(cumulative.Metadata, 50);

        //assert
        sut.Items.Count.Should().Be(1);
        sut.Items.Should().NotContain(cumulative);
    }

    [Fact]
    public void Player_removes_part_of_cumulative_from_container()
    {
        //arrange
        var innerContainer = ItemTestData.CreateContainer(10);
        var cumulative = ItemTestData.CreateCumulativeItem(1, 100);
        var item3 = ItemTestData.CreateWeaponItem(1);

        innerContainer.AddItem(item3);

        var children = new List<IItem>
        {
            cumulative,
            innerContainer
        };

        var sut = ItemTestData.CreateContainer(5, children: children);

        //act
        sut.RemoveItem(cumulative.Metadata, 50);

        //assert
        sut.Items.Count.Should().Be(2);
        sut.Items.Should().Contain(cumulative);
        cumulative.Amount.Should().Be(50);
    }

    [Fact]
    public void Player_removes_cumulative_from_container_and_inner_container()
    {
        //arrange
        var innerContainer = ItemTestData.CreateContainer(10);
        var cumulative = ItemTestData.CreateCumulativeItem(1, 30);
        var cumulative2 = ItemTestData.CreateCumulativeItem(1, 50);
        var item3 = ItemTestData.CreateWeaponItem(1);

        innerContainer.AddItem(item3);
        innerContainer.AddItem(cumulative2);

        var children = new List<IItem>
        {
            cumulative,
            innerContainer
        };

        var sut = ItemTestData.CreateContainer(5, children: children);

        //act
        sut.RemoveItem(cumulative.Metadata, 80);

        //assert
        sut.Items.Count.Should().Be(1);
        sut.Items.Should().NotContain(cumulative);
        innerContainer.Items.Should().NotContain(cumulative2);
    }

    [Fact]
    public void Player_removes_cumulative_from_container_and_inner_container_but_remains_20()
    {
        //arrange
        var innerContainer = ItemTestData.CreateContainer(10);
        var cumulative = ItemTestData.CreateCumulativeItem(1, 30);
        var cumulative2 = ItemTestData.CreateCumulativeItem(1, 70);
        var item3 = ItemTestData.CreateWeaponItem(1);

        innerContainer.AddItem(item3);
        innerContainer.AddItem(cumulative2);

        var children = new List<IItem>
        {
            cumulative,
            innerContainer
        };

        var sut = ItemTestData.CreateContainer(5, children: children);

        //act
        sut.RemoveItem(cumulative.Metadata, 80);

        //assert
        sut.Items.Count.Should().Be(1);
        sut.Items.Should().NotContain(cumulative);
        innerContainer.Items.Should().Contain(cumulative2);
        cumulative2.Amount.Should().Be(20);
    }

    #endregion
}