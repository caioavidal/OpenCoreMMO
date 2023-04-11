using FluentAssertions;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Items.Services;
using NeoServer.Game.Systems.SafeTrade;
using NeoServer.Game.Systems.SafeTrade.Operations;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.Tests.Helpers.Map;
using NeoServer.Game.Tests.Helpers.Player;
using NeoServer.Game.World.Map;
using NeoServer.Game.World.Models.Tiles;

namespace NeoServer.Game.Systems.Tests.SafeTrade;

public class PreTradeValidationTests
{
    [Fact]
    public void Trade_fails_when_player_tries_to_trade_with_himself()
    {
        //arrange
        var map = new Map(new World.World());
        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);
        
        var player = PlayerTestDataBuilder.Build();
        var item = ItemTestData.CreateWeaponItem(id: 1);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        var result = tradeSystem.Request(player, player, item);

        //assert
        result.Should().BeFalse();
        error.Should().Be("Select a player to trade with.");
    }

    [Fact]
    public void Trade_fails_when_player_is_already_trading()
    {
        //arrange
        var map = new Map(new World.World());
        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build();
        var secondPlayer = PlayerTestDataBuilder.Build();

        var item = ItemTestData.CreateWeaponItem(id: 1);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        tradeSystem.Request(player, secondPlayer, item);

        //act
        var result = tradeSystem.Request(player, secondPlayer, item);

        //assert
        result.Should().BeFalse();
        error.Should().Be("You are already trading.");
    }

    [Fact]
    public void Trade_fails_when_has_no_items()
    {
        //arrange
        var map = new Map(new World.World());
        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build();
        var secondPlayer = PlayerTestDataBuilder.Build();

        //act
        var result = tradeSystem.Request(player, secondPlayer, null);

        //assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Trade_fails_when_it_has_more_than_255_items()
    {
        //arrange
        var map = new Map(new World.World());
        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build();
        var secondPlayer = PlayerTestDataBuilder.Build();

        var backpack1 = ItemTestData.CreatePickupableContainer(capacity: 255);
        var backpack2 = ItemTestData.CreatePickupableContainer(capacity: 255);

        Enumerable.Range(1, 200).ToList().ForEach(x => backpack1.AddItem(ItemTestData.CreateWeaponItem(id: (ushort)x)));
        backpack1.AddItem(backpack2);

        Enumerable.Range(1, 200).ToList().ForEach(x => backpack2.AddItem(ItemTestData.CreateWeaponItem(id: (ushort)x)));

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        var result = tradeSystem.Request(player, secondPlayer, backpack1);

        //assert
        result.Should().BeFalse();
        error.Should().Be("You cannot trade more than 255 items at once.");
    }

    [Fact]
    public void Trade_fails_when_item_is_already_being_traded()
    {
        //arrange
        var map = new Map(new World.World());
        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build();
        var secondPlayer = PlayerTestDataBuilder.Build();
        var thirdPlayer = PlayerTestDataBuilder.Build();

        var item = ItemTestData.CreateWeaponItem(id: 1);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        tradeSystem.Request(player, secondPlayer, item);

        //act
        var result = tradeSystem.Request(thirdPlayer, secondPlayer, item);

        //assert
        result.Should().BeFalse();
        error.Should().Be("This item is already being traded.");
    }

    [Fact]
    public void Trade_fails_when_player_is_not_close_to_item()
    {
        //arrange
        var map = new Map(new World.World());
        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var tile = MapTestDataBuilder.CreateTile(new Location(100, 100, 7));
        var tile2 = (DynamicTile)MapTestDataBuilder.CreateTile(new Location(100, 102, 7));

        var player = PlayerTestDataBuilder.Build();
        var secondPlayer = PlayerTestDataBuilder.Build();

        var item = ItemTestData.CreateWeaponItem(id: 1);

        tile.AddItem(item);
        tile2.AddCreature(player);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        var result = tradeSystem.Request(player, secondPlayer, item);

        //assert
        result.Should().BeFalse();
        error.Should().Be("You are not close to the item.");
    }

    [Fact]
    public void Trade_fails_when_player_is_not_close_enough_to_second_player()
    {
        //arrange
        var map = new Map(new World.World());
        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);
        
        var tile = (DynamicTile)MapTestDataBuilder.CreateTile(new Location(100, 100, 7));
        var tile2 = (DynamicTile)MapTestDataBuilder.CreateTile(new Location(100, 103, 7));

        var player = PlayerTestDataBuilder.Build();
        var secondPlayer = PlayerTestDataBuilder.Build(name: "Player2");

        var item = ItemTestData.CreateWeaponItem(id: 1);

        tile.AddCreature(player);
        tile2.AddCreature(secondPlayer);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        var result = tradeSystem.Request(player, secondPlayer, item);

        //assert
        result.Should().BeFalse();
        error.Should().Be("Player2 tells you to move close.");
    }

    [Fact]
    public void Trade_fails_when_there_is_a_wall_in_middle_of_two_players()
    {
        //arrange
        var tile = (DynamicTile)MapTestDataBuilder.CreateTile(new Location(100, 100, 7));
        var middleTile = (DynamicTile)MapTestDataBuilder.CreateTile(new Location(100, 101, 7));
        var tile2 = (DynamicTile)MapTestDataBuilder.CreateTile(new Location(100, 102, 7));

        var map = MapTestDataBuilder.Build(tile, middleTile, tile2);
        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build();
        var secondPlayer = PlayerTestDataBuilder.Build(name: "Player2");

        var item = ItemTestData.CreateWeaponItem(id: 1);
        var wall = ItemTestData.CreateTopItem(id: 2, topOrder: 1);

        wall.Metadata.Flags.Add(ItemFlag.Unpassable);
        wall.Metadata.Flags.Add(ItemFlag.BlockProjectTile);

        middleTile.AddItem(wall);

        tile.AddCreature(player);
        tile2.AddCreature(secondPlayer);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        var result = tradeSystem.Request(player, secondPlayer, item);

        //assert
        result.Should().BeFalse();
        error.Should().Be("Player2 tells you to move close.");
    }
    
    [Fact]
    public void Trade_fails_when_second_player_is_already_trading()
    {
        //arrange
        var map = new Map(new World.World());
        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build();
        var secondPlayer = PlayerTestDataBuilder.Build();
        var thirdPlayer = PlayerTestDataBuilder.Build();

        var item = ItemTestData.CreateWeaponItem(id: 1);
        var item2 = ItemTestData.CreateWeaponItem(id: 1);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        tradeSystem.Request(secondPlayer, thirdPlayer, item);

        //act
        var result = tradeSystem.Request(player, secondPlayer, item2);

        //assert
        result.Should().BeFalse();
        error.Should().Be("This person is already trading.");
    }
    
    [Fact]
    public void Trade_fails_when_item_traded_is_not_pickupable()
    {
        //arrange
        var map = new Map(new World.World());
        var tradeSystem = new SafeTradeSystem(new TradeItemExchanger(new ItemRemoveService(map)), map);

        var player = PlayerTestDataBuilder.Build();
        var secondPlayer = PlayerTestDataBuilder.Build();

        var item = ItemTestData.CreateUnpassableItem(id: 1);

        var error = string.Empty;
        OperationFailService.OnOperationFailed += (_, message) => error = message;

        //act
        var result = tradeSystem.Request(player, secondPlayer, item);

        //assert
        result.Should().BeFalse();
        error.Should().Be("Item cannot be traded.");
    }
}