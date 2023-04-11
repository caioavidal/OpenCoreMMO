using FluentAssertions;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Items.Services;
using NeoServer.Game.Systems.SafeTrade;
using NeoServer.Game.Systems.SafeTrade.Operations;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.Tests.Helpers.Map;
using NeoServer.Game.Tests.Helpers.Player;
using NeoServer.Game.World.Models.Tiles;

namespace NeoServer.Game.Systems.Tests.SafeTrade;

public class TradeCancellationTests
{

    #region Player event cancellation

     [Fact]
    public void Trade_is_cancelled_when_player_moves_2_sqms_way_from_second_player()
    {
        //arrange

        var map = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 8);

        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build();
        var secondPlayer = PlayerTestDataBuilder.Build();

        ((DynamicTile)map[100, 100, 7]).AddCreature(secondPlayer);
        ((DynamicTile)map[101, 100, 7]).AddCreature(player);

        var item = ItemTestData.CreateWeaponItem(id: 1);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        tradeSystem.Request(player, secondPlayer, item);

        player.WalkTo(new Location(103, 100, 7));
        map.MoveCreature(player);

        //assert
        AssertTradeIsCancelled(tradeSystem, map, error, player);
        AssertTradeIsCancelled(tradeSystem, map, error, secondPlayer);
    }

    [Fact]
    public void Trade_is_cancelled_when_player_logs_out()
    {
        //arrange
        var map = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 8);

        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build();
        var secondPlayer = PlayerTestDataBuilder.Build();

        ((DynamicTile)map[100, 100, 7]).AddCreature(secondPlayer);
        ((DynamicTile)map[101, 100, 7]).AddCreature(player);

        var item = ItemTestData.CreateWeaponItem(id: 1);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        tradeSystem.Request(player, secondPlayer, item);
        player.Logout();

        //assert
        AssertTradeIsCancelled(tradeSystem, map, error, secondPlayer);
    }

    [Fact]
    public void Trade_is_cancelled_when_player_dies()
    {
        //arrange
        var map = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 8);

        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build(hp: 10);
        var secondPlayer = PlayerTestDataBuilder.Build();

        ((DynamicTile)map[100, 100, 7]).AddCreature(secondPlayer);
        ((DynamicTile)map[101, 100, 7]).AddCreature(player);

        var item = ItemTestData.CreateWeaponItem(id: 1);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        tradeSystem.Request(player, secondPlayer, item);
        player.ReceiveAttack(secondPlayer, new CombatDamage(100, DamageType.Melee));

        //assert
        AssertTradeIsCancelled(tradeSystem, map, error, secondPlayer);
    }
    
    [Fact]
    public void Trade_is_cancelled_when_player_moves_away_from_the_traded_item()
    {
        //arrange
        var map = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 8);

        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build(hp: 10);
        var secondPlayer = PlayerTestDataBuilder.Build();

        ((DynamicTile)map[101, 100, 7]).AddCreature(secondPlayer);
        ((DynamicTile)map[100, 100, 7]).AddCreature(player);

        var item = ItemTestData.CreateWeaponItem(id: 1);
        ((DynamicTile)map[100, 100, 7]).AddItem(item);
        
        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        tradeSystem.Request(player, secondPlayer, item);
        player.WalkTo(Direction.East, Direction.East);
        
        map.MoveCreature(player);
        map.MoveCreature(player);

        //assert
        AssertTradeIsCancelled(tradeSystem, map, error, secondPlayer);
    }

    #endregion
    
    #region Item movement cancellation

    [Fact]
    public void Trade_is_cancelled_when_item_is_removed_from_tile()
    {
        //arrange
        var map = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 8);

        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build(hp: 10);
        var secondPlayer = PlayerTestDataBuilder.Build();

        ((DynamicTile)map[100, 100, 7]).AddCreature(secondPlayer);
        ((DynamicTile)map[101, 100, 7]).AddCreature(player);

        var item = ItemTestData.CreateWeaponItem(id: 1);

        ((DynamicTile)map[101, 100, 7]).AddItem(item);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        tradeSystem.Request(player, secondPlayer, item);
        ((DynamicTile)map[101, 100, 7]).RemoveItem(item);

        //assert
        AssertTradeIsCancelled(tradeSystem, map, error, secondPlayer);
    }

    [Fact]
    public void Trade_is_cancelled_when_item_is_removed_from_inventory()
    {
        //arrange
        var map = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 8);

        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var inventory = InventoryTestDataBuilder.GenerateInventory();
        var player = PlayerTestDataBuilder.Build(hp: 10, capacity: uint.MaxValue, inventoryMap: inventory);
        var secondPlayer = PlayerTestDataBuilder.Build();

        ((DynamicTile)map[100, 100, 7]).AddCreature(secondPlayer);
        ((DynamicTile)map[101, 100, 7]).AddCreature(player);

        var item = inventory[Slot.Left].Item;

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        tradeSystem.Request(player, secondPlayer, item);
        player.Inventory.RemoveItem(Slot.Left, 1);

        //assert
        AssertTradeIsCancelled(tradeSystem, map, error, secondPlayer);
    }

    [Fact]
    public void Trade_is_cancelled_when_item_is_removed_from_inventory_backpack()
    {
        //arrange
        var map = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 8);

        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var inventory = InventoryTestDataBuilder.GenerateInventory();
        var player = PlayerTestDataBuilder.Build(hp: 10, capacity: uint.MaxValue, inventoryMap: inventory);
        var secondPlayer = PlayerTestDataBuilder.Build();

        ((DynamicTile)map[100, 100, 7]).AddCreature(secondPlayer);
        ((DynamicTile)map[101, 100, 7]).AddCreature(player);

        var backpack = (IContainer)inventory[Slot.Backpack].Item;
        var item = ItemTestData.CreateWeaponItem(id: 1);

        backpack.AddItem(item);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        tradeSystem.Request(player, secondPlayer, backpack);
        backpack.RemoveItem(item, 1);

        //assert
        AssertTradeIsCancelled(tradeSystem, map, error, player);
        AssertTradeIsCancelled(tradeSystem, map, error, secondPlayer);
    }

    [Fact]
    public void Trade_is_cancelled_when_item_is_added_to_traded_backpack()
    {
        //arrange
        var map = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 8);

        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var inventory = InventoryTestDataBuilder.GenerateInventory();
        var player = PlayerTestDataBuilder.Build(hp: 10, capacity: uint.MaxValue, inventoryMap: inventory);
        var secondPlayer = PlayerTestDataBuilder.Build();

        ((DynamicTile)map[100, 100, 7]).AddCreature(secondPlayer);
        ((DynamicTile)map[101, 100, 7]).AddCreature(player);

        var backpack = (IContainer)inventory[Slot.Backpack].Item;
        var item = ItemTestData.CreateWeaponItem(id: 1);
        var item3 = ItemTestData.CreateWeaponItem(id: 1);

        backpack.AddItem(item);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        tradeSystem.Request(player, secondPlayer, backpack);
        backpack.AddItem(item3, 1);

        //assert
        AssertTradeIsCancelled(tradeSystem, map, error, player);
        AssertTradeIsCancelled(tradeSystem, map, error, secondPlayer);
    }

    [Fact]
    public void Trade_is_cancelled_when_item_is_added_to_traded_backpack_in_the_ground()
    {
        //arrange
        var map = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 8);

        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build(hp: 10, capacity: uint.MaxValue);
        var secondPlayer = PlayerTestDataBuilder.Build();

        var backpack = ItemTestData.CreateBackpack();

        ((DynamicTile)map[100, 100, 7]).AddCreature(secondPlayer);
        ((DynamicTile)map[101, 100, 7]).AddCreature(player);
        ((DynamicTile)map[100, 100, 7]).AddItem(backpack);

        var item = ItemTestData.CreateWeaponItem(id: 1);
        var item3 = ItemTestData.CreateWeaponItem(id: 1);

        backpack.AddItem(item);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        tradeSystem.Request(player, secondPlayer, backpack);
        backpack.AddItem(item3, 1);

        //assert
        AssertTradeIsCancelled(tradeSystem, map, error, player);
        AssertTradeIsCancelled(tradeSystem, map, error, secondPlayer);
    }
    
    [Fact]
    public void Trade_is_cancelled_when_item_is_removed_from_traded_backpack_in_the_ground()
    {
        //arrange
        var map = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 8);

        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build(hp: 10, capacity: uint.MaxValue);
        var secondPlayer = PlayerTestDataBuilder.Build();

        var backpack = ItemTestData.CreateBackpack();

        ((DynamicTile)map[100, 100, 7]).AddCreature(secondPlayer);
        ((DynamicTile)map[101, 100, 7]).AddCreature(player);
        ((DynamicTile)map[100, 100, 7]).AddItem(backpack);

        var item = ItemTestData.CreateWeaponItem(id: 1);

        backpack.AddItem(item);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        tradeSystem.Request(player, secondPlayer, backpack);
        backpack.RemoveItem(item, 1);

        //assert
        AssertTradeIsCancelled(tradeSystem, map, error, player);
        AssertTradeIsCancelled(tradeSystem, map, error, secondPlayer);
    }

    #endregion

    #region Item consumption cancellation

    [Fact]
    public void Trade_is_cancelled_when_item_is_consumed_from_tile()
    {
        //arrange
        var map = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 8);

        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build(hp: 10);
        var secondPlayer = PlayerTestDataBuilder.Build();

        ((DynamicTile)map[100, 100, 7]).AddCreature(secondPlayer);
        ((DynamicTile)map[101, 100, 7]).AddCreature(player);

        var item = ItemTestData.CreateFood(id: 1, amount:5);

        ((DynamicTile)map[101, 100, 7]).AddItem(item);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        tradeSystem.Request(player, secondPlayer, item);
        player.Use(item, player);

        //assert
        AssertTradeIsCancelled(tradeSystem, map, error, secondPlayer);
    }

    #endregion

    private void AssertTradeIsCancelled(SafeTradeSystem tradeSystem, IMap map, string error, IPlayer player)
    {
        var secondPlayer = PlayerTestDataBuilder.Build();

        var x = (ushort)(player.Location.X + 1);

        var tile = (DynamicTile)map[x, player.Location.Y, player.Location.Z];
        tile.AddCreature(secondPlayer);
        var item = ItemTestData.CreateWeaponItem(id: 1);

        tile.AddItem(item);

        error.Should().Be("Trade is cancelled.");
        var result = tradeSystem.Request(player, secondPlayer, item);

        result.Should().BeTrue();
    }
}