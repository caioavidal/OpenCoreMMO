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
    [Fact]
    public void Player_moves_item_from_ground_to_inventory()
    {
        //arrange
        var inventory = InventoryTestDataBuilder.Build(PlayerTestDataBuilder.Build(capacity: 1000),
            new Dictionary<Slot, Tuple<IPickupable, ushort>>());

        var item = ItemTestData.CreateWeaponItem(100);

        ITile tile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, null, Array.Empty<IItem>(),
            new IItem[] { item });

        var sut = new MoveService();

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

        var sut = new MoveService();

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

        var sut = new MoveService();

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

        var sut = new MoveService();

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

        var sut = new MoveService();

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

        var sut = new MoveService();

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

        var sut = new MoveService();

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
        
        var sut = new MoveService();

        //act
        var result = sut.Move(ammo, container,inventory, 1,(byte)ammo.Location.ContainerSlot, (byte)Slot.Ammo);

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
        
        var sut = new MoveService();

        //act
        var result = sut.Move(ammo, backpack,inventory, 1,(byte)ammo.Location.ContainerSlot, (byte)Slot.Ammo);

        //assert
        Assert.True(result.IsSuccess);
        Assert.Equal((uint)20, backpack.FreeSlotsCount);
        Assert.Equal(ammo, inventory[Slot.Ammo]);
        Assert.Equal(1, inventory[Slot.Ammo].Amount);
    }

    #region Tile tests

    [Fact]
    public void Player_moves_item_from_tile_to_another_tile()
    {
        //arrange
        ITile fromTile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, null, Array.Empty<IItem>(), Array.Empty<IItem>());
        ITile dest = new DynamicTile(new Coordinate(102, 100, 7), TileFlag.None, null, Array.Empty<IItem>(), Array.Empty<IItem>());

        var item = ItemTestData.CreateRegularItem(100);
        fromTile.AddItem(item);
        
        var sut = new MoveService();

        //act
        var result = sut.Move(item, fromTile, dest, 1, 0, 0);

        //assert
        Assert.True(result.IsSuccess);
        Assert.Null(fromTile.TopItemOnStack);
        Assert.Equal(item, dest.TopItemOnStack);
    }

    #endregion
}