using System;
using System.Collections.Generic;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Services;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.World.Models.Tiles;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players;

public class PlayerMoveTests
{
    #region Inventory tests

    [Fact]
    public void Player_moves_item_from_ground_to_inventory()
    {
        //arrange
        var inventory = InventoryTestDataBuilder.Build(PlayerTestDataBuilder.Build(capacity: 1000),
            new Dictionary<Slot, Tuple<IPickupable, ushort>>());

        var item = ItemTestData.CreateWeaponItem(100);

        ITile tile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            new IItem[] { item });

        var sut = new ItemMovementService();

        //act
        var result = sut.Move(item, tile, inventory, 1, 0, (byte)Slot.Left);

        //assert
        Assert.True(result.IsSuccess);
        Assert.Equal(inventory[Slot.Left], item);
        Assert.Null(tile.TopItemOnStack);
    }

    [Fact]
    public void Player_swaps_item_from_ground_to_inventory()
    {
        //arrange
        var dictionary = new Dictionary<Slot, Tuple<IPickupable, ushort>>();

        var itemOnInventory = ItemTestData.CreateWeaponItem(200, "axe");
        dictionary.Add(Slot.Left, new Tuple<IPickupable, ushort>(itemOnInventory, 200));

        var inventory = InventoryTestDataBuilder.Build(PlayerTestDataBuilder.Build(capacity: 1000), dictionary);
        var item = ItemTestData.CreateWeaponItem(100);
        ITile tile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            new IItem[] { item });

        var sut = new ItemMovementService();

        //act
        var result = sut.Move(item, tile, inventory, 1, 0, (byte)Slot.Left);

        //assert
        Assert.True(result.IsSuccess);
        Assert.Equal(inventory[Slot.Left], item);
        Assert.Equal(itemOnInventory, tile.TopItemOnStack);
    }

    [Fact]
    public void Player_moves_cumulative_from_ground_to_inventory()
    {
        //arrange
        var inventory = InventoryTestDataBuilder.Build(PlayerTestDataBuilder.Build(capacity: 1000),
            new Dictionary<Slot, Tuple<IPickupable, ushort>>());

        var item = ItemTestData.CreateAmmo(100, 20);
        ITile tile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            new IItem[] { item });

        var sut = new ItemMovementService();

        //act
        var result = sut.Move(tile.TopItemOnStack, tile, inventory, 20, 0, (byte)Slot.Ammo);

        //assert
        Assert.True(result.IsSuccess);
        Assert.Equal(20, inventory[Slot.Ammo].Amount);

        Assert.Null(tile.TopItemOnStack);
    }

    [Fact]
    public void Player_moves_cumulative_from_ground_to_exiting_item_on_inventory()
    {
        //arrange
        var itemOnInventory = ItemTestData.CreateAmmo(100, 51);
        var inventory = InventoryTestDataBuilder.Build(PlayerTestDataBuilder.Build(capacity: 1000),
            new Dictionary<Slot, Tuple<IPickupable, ushort>>
            {
                { Slot.Ammo, new Tuple<IPickupable, ushort>(itemOnInventory, 100) }
            });

        var item = ItemTestData.CreateAmmo(100, 100);
        ITile tile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            new IItem[] { item });

        var sut = new ItemMovementService();

        //act
        var result = sut.Move(tile.TopItemOnStack, tile, inventory, 100, 0, (byte)Slot.Ammo);

        //assert
        Assert.False(result.IsSuccess);
        Assert.Equal(InvalidOperation.NotEnoughRoom, result.Error);
        Assert.Equal(100, inventory[Slot.Ammo].Amount);

        Assert.Equal(51, tile.TopItemOnStack.Amount);
    }


    [Fact]
    public void Player_cannot_move_cumulative_from_ground_to_inventory_when_full()
    {
        //arrange
        var itemOnInventory = ItemTestData.CreateWeaponItem(100);
        var inventory = InventoryTestDataBuilder.Build(PlayerTestDataBuilder.Build(capacity: 1000),
            new Dictionary<Slot, Tuple<IPickupable, ushort>>
            {
                { Slot.Left, new Tuple<IPickupable, ushort>(itemOnInventory, 100) }
            });

        var item = ItemTestData.CreateThrowableDistanceItem(200, 100);
        ITile tile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            new IItem[] { item });

        var sut = new ItemMovementService();

        //act
        var result = sut.Move(tile.TopItemOnStack, tile, inventory, 100, 0, (byte)Slot.Ammo);

        //assert
        Assert.False(result.IsSuccess);
        Assert.Equal(itemOnInventory, inventory[Slot.Left]);

        Assert.Equal(item, tile.TopItemOnStack);
        Assert.Equal(100, tile.TopItemOnStack.Amount);
    }

    [Fact]
    public void Player_moves_cumulative_from_ground_to_inventory_backpack()
    {
        //arrange
        var backpack = ItemTestData.CreateBackpack();
        var inventory = InventoryTestDataBuilder.Build(PlayerTestDataBuilder.Build(capacity: 1000),
            new Dictionary<Slot, Tuple<IPickupable, ushort>>
            {
                { Slot.Backpack, new Tuple<IPickupable, ushort>(backpack, 100) }
            });

        var item = ItemTestData.CreateAmmo(200, 100);
        ITile tile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            new IItem[] { item });

        var sut = new ItemMovementService();

        //act
        var result = sut.Move(tile.TopItemOnStack, tile, inventory, 100, 0, (byte)Slot.Backpack);

        //assert
        Assert.True(result.IsSuccess);
        Assert.Equal(item, ((IContainer)inventory[Slot.Backpack])[item.Location.ContainerSlot]);

        Assert.Null(tile.TopItemOnStack);
        Assert.Equal(100, ((IContainer)inventory[Slot.Backpack])[item.Location.ContainerSlot].Amount);
    }

    [Fact]
    public void Player_moves_item_from_inventory_to_tile()
    {
        //arrange
        var item = ItemTestData.CreateBodyEquipmentItem(100, "", "shield");

        var inventory = InventoryTestDataBuilder.Build(PlayerTestDataBuilder.Build(capacity: 1000),
            new Dictionary<Slot, Tuple<IPickupable, ushort>>
            {
                { Slot.Right, new Tuple<IPickupable, ushort>(item, 100) }
            });
        ITile tile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            new IItem[] { item });

        var sut = new ItemMovementService();

        //act
        var result = sut.Move(item, inventory, tile, 1, (byte)Slot.Right, 0);

        //assert
        Assert.True(result.IsSuccess);
        Assert.Null(inventory[Slot.Right]);
        Assert.Equal(item, tile.TopItemOnStack);
    }

    [Fact]
    public void Player_moves_ammo_from_container_to_inventory()
    {
        //arrange
        var container = ItemTestData.CreateContainer(2);
        var ammo = ItemTestData.CreateAmmo(100, 1);
        container.AddItem(ammo);

        var inventory = InventoryTestDataBuilder.Build(PlayerTestDataBuilder.Build(capacity: 1000),
            new Dictionary<Slot, Tuple<IPickupable, ushort>>());

        var sut = new ItemMovementService();

        //act
        var result = sut.Move(ammo, container, inventory, 1, (byte)ammo.Location.ContainerSlot, (byte)Slot.Ammo);

        //assert
        Assert.True(result.IsSuccess);
        Assert.Equal((uint)2, container.FreeSlotsCount);
        Assert.Equal(ammo, inventory[Slot.Ammo]);
        Assert.Equal(1, inventory[Slot.Ammo].Amount);
    }


    [Fact]
    public void Player_moves_ammo_from_backpack_to_inventory()
    {
        //arrange
        var backpack = ItemTestData.CreateBackpack();
        var ammo = ItemTestData.CreateAmmo(100, 1);
        backpack.AddItem(ammo);

        var inventory = InventoryTestDataBuilder.Build(PlayerTestDataBuilder.Build(capacity: 1000),
            new Dictionary<Slot, Tuple<IPickupable, ushort>>
            {
                { Slot.Backpack, new Tuple<IPickupable, ushort>(backpack, 100) }
            });

        var sut = new ItemMovementService();

        //act
        var result = sut.Move(ammo, backpack, inventory, 1, (byte)ammo.Location.ContainerSlot, (byte)Slot.Ammo);

        //assert
        Assert.True(result.IsSuccess);
        Assert.Equal((uint)20, backpack.FreeSlotsCount);
        Assert.Equal(ammo, inventory[Slot.Ammo]);
        Assert.Equal(1, inventory[Slot.Ammo].Amount);
    }

    #endregion

    #region Tile tests

    [Fact]
    public void Player_moves_item_from_tile_to_another_tile()
    {
        //arrange
        ITile fromTile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            Array.Empty<IItem>());
        ITile dest = new DynamicTile(new Coordinate(102, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            Array.Empty<IItem>());

        var item = ItemTestData.CreateRegularItem(100);
        fromTile.AddItem(item);

        var sut = new ItemMovementService();

        //act
        var result = sut.Move(item, fromTile, dest, 1, 0, 0);

        //assert
        Assert.True(result.IsSuccess);
        Assert.Null(fromTile.TopItemOnStack);
        Assert.Equal(item, dest.TopItemOnStack);
    }

    [Fact]
    public void Player_moves_cumulative_item_from_tile_to_another_tile()
    {
        //arrange
        ITile fromTile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            Array.Empty<IItem>());
        ITile dest = new DynamicTile(new Coordinate(102, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            Array.Empty<IItem>());

        var item = ItemTestData.CreateAmmo(100, 100);

        fromTile.AddItem(item);
        var sut = new ItemMovementService();

        //act
        var result = sut.Move(item, fromTile, dest, 80, 0, 0);

        //assert
        Assert.True(result.IsSuccess);

        Assert.Equal(100, dest.TopItemOnStack.ClientId);
        Assert.Equal(item, fromTile.TopItemOnStack);

        Assert.Equal(20, ((ICumulative)fromTile.TopItemOnStack).Amount);
        Assert.Equal(80, ((ICumulative)dest.TopItemOnStack).Amount);
    }

    [Fact]
    public void Player_moves_half_of_cumulative_item_from_tile_to_another_tile()
    {
        //arrange
        ITile fromTile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            Array.Empty<IItem>());
        ITile dest = new DynamicTile(new Coordinate(102, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            Array.Empty<IItem>());

        var item = ItemTestData.CreateAmmo(100, 100);

        fromTile.AddItem(item);
        var sut = new ItemMovementService();

        //act
        var result = sut.Move(item, fromTile, dest, 50, 0, 0);

        //assert
        Assert.True(result.IsSuccess);

        Assert.Equal(100, dest.TopItemOnStack.ClientId);
        Assert.Equal(item, fromTile.TopItemOnStack);

        Assert.Equal(50, ((ICumulative)fromTile.TopItemOnStack).Amount);
        Assert.Equal(50, ((ICumulative)dest.TopItemOnStack).Amount);
    }

    [Fact]
    public void Player_moves_part_of_cumulative_item_from_tile_to_join_in_another_tile()
    {
        //arrange
        ITile fromTile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            Array.Empty<IItem>());
        ITile dest = new DynamicTile(new Coordinate(102, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            new IItem[1] { ItemTestData.CreateAmmo(100, 50) });

        var item = ItemTestData.CreateAmmo(100, 100);
        fromTile.AddItem(item);
        var sut = new ItemMovementService();

        //act
        var result = sut.Move(item, fromTile, dest, 40, 0, 0);

        //assert
        Assert.True(result.IsSuccess);

        Assert.Equal(100, dest.TopItemOnStack.ClientId);
        Assert.Equal(item, fromTile.TopItemOnStack);

        Assert.Equal(60, ((ICumulative)fromTile.TopItemOnStack).Amount);
        Assert.Equal(90, ((ICumulative)dest.TopItemOnStack).Amount);

        //act
        result = sut.Move(item, fromTile, dest, 60, 0, 0);

        //assert
        Assert.True(result.IsSuccess);

        Assert.Equal(100, dest.TopItemOnStack.ClientId);
        Assert.Null(fromTile.TopItemOnStack);

        Assert.Equal(50, ((ICumulative)dest.TopItemOnStack).Amount);
    }

    #endregion

    #region Container tests

    [Fact]
    public void Player_adds_cumulative_item_to_child_container_joins_or_return_full_when_exceeds()
    {
        //arrange
        var fromContainer = ItemTestData.CreateContainer(2);
        var child = ItemTestData.CreateContainer(1);

        var itemOnChild = ItemTestData.CreateCumulativeItem(100, 50);
        child.AddItem(itemOnChild);

        var item = ItemTestData.CreateCumulativeItem(100, 100);
        fromContainer.AddItem(child);
        fromContainer.AddItem(item);

        var sut = new ItemMovementService();

        //act
        var result = sut.Move(item, fromContainer, child, 70,(byte)item.Location.ContainerSlot,
            (byte)child.Location.ContainerSlot);

        //assert
        Assert.False(result.IsSuccess);
        Assert.Equal(InvalidOperation.IsFull, result.Error);

        Assert.Equal(100, itemOnChild.Amount);
        Assert.Equal(50, item.Amount);
    }
    
    [Fact]
    public void Player_moves_cumulative_item_from_container_to_child()
    {
        //arrange
        var fromContainer = ItemTestData.CreateContainer(2);
        var child = ItemTestData.CreateContainer(1);
        var item = ItemTestData.CreateCumulativeItem(100, 40);

        fromContainer.AddItem(child);
        fromContainer.AddItem(item);

        var eventCalled = false;
        var childEventCalled = false;

        fromContainer.OnItemRemoved += (_, _) => { eventCalled = true; };
        child.OnItemAdded += (_, _) => { childEventCalled = true; };
        
        var sut = new ItemMovementService();

        //act
        sut.Move(item, fromContainer, child,40, (byte)item.Location.ContainerSlot, (byte)child.Location.ContainerSlot);

        //assert
        Assert.True(eventCalled);
        Assert.True(childEventCalled);

        Assert.Single(fromContainer.Items);
        Assert.Equal(40, ((ICumulative)child[0]).Amount);
    }
    
    [Fact]
    public void Player_moves_regular_item_from_container_to_child()
    {
        //arrange
        var fromContainer = ItemTestData.CreateContainer(2);
        var child = ItemTestData.CreateContainer(1);
        var item = ItemTestData.CreateBodyEquipmentItem(100, "head");

        fromContainer.AddItem(child);
        fromContainer.AddItem(item);

        var eventCalled = false;
        var childEventCalled = false;

        fromContainer.OnItemRemoved += (_, _) => { eventCalled = true; };
        child.OnItemAdded += (_, _) => { childEventCalled = true; };
        var sut = new ItemMovementService();

        //act
        sut.Move(item, fromContainer, child,1, (byte)item.Location.ContainerSlot, (byte)child.Location.ContainerSlot);

        //assert
        Assert.True(eventCalled);
        Assert.True(childEventCalled);

        Assert.Single(fromContainer.Items);
        Assert.Equal(100, child[0].ClientId);
    }
    
    [Fact]
    public void Player_cannot_move_cumulative_to_child_when_it_is_already_full()
    {
        //arrange
        var fromContainer = ItemTestData.CreateContainer(2);
        var child = ItemTestData.CreateContainer(1);
        var item = ItemTestData.CreateCumulativeItem(100, 40);
        var item2 = ItemTestData.CreateCumulativeItem(100, 100);

        fromContainer.AddItem(child);
        fromContainer.AddItem(item);
        child.AddItem(item2);
        var sut = new ItemMovementService();

        //act
        var result = sut.Move(item,fromContainer, child,40, (byte)item.Location.ContainerSlot,
            (byte)child.Location.ContainerSlot);

        //assert
        Assert.Equal(InvalidOperation.IsFull, result.Error);
        Assert.Equal(2, fromContainer.Items.Count);
        Assert.Equal(100, ((ICumulative)child[0]).Amount);
    }
    
    [Fact]
    public void Player_moves_backpack_to_first_slot_of_another_backpack()
    {
        //arrange
        var anotherBackpack = ItemTestData.CreateContainer(2);
        var bp1 = ItemTestData.CreateContainer(2);
        var item = ItemTestData.CreateBackpack();

        bp1.AddItem(item);
        var sut = new ItemMovementService();

        //act
        sut.Move(item, bp1, anotherBackpack, 1, 0, 0);

        //assert
        Assert.Single(anotherBackpack.Items);
        Assert.Equal(item, anotherBackpack[0]);
    }
    
    [Fact]
    public void Player_moves_cumulative_item_from_container_to_join_child()
    {
        //arrange
        var fromContainer = ItemTestData.CreateContainer(2);
        var child = ItemTestData.CreateContainer(1);
        var item = ItemTestData.CreateCumulativeItem(100, 40);
        var item2 = ItemTestData.CreateCumulativeItem(100, 20);

        fromContainer.AddItem(child);
        fromContainer.AddItem(item);
        child.AddItem(item2);

        var eventCalled = false;
        var childEventCalled = false;

        fromContainer.OnItemUpdated += (_, _, _) => { eventCalled = true; };
        child.OnItemUpdated += (_, _, _) => { childEventCalled = true; };

        var sut = new ItemMovementService();

        //act
        sut.Move(item, fromContainer, child,20, (byte)item.Location.ContainerSlot,
            (byte)child.Location.ContainerSlot);

        //assert
        Assert.True(eventCalled);
        Assert.True(childEventCalled);

        Assert.Equal(2, fromContainer.Items.Count);
        Assert.Equal(20, ((ICumulative)fromContainer[(byte)item.Location.ContainerSlot]).Amount);
        Assert.Equal(40, ((ICumulative)child[(byte)item2.Location.ContainerSlot]).Amount);
    }
    
    #endregion
}