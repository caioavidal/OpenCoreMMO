using FluentAssertions;
using NeoServer.Data.InMemory.DataStores;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.Tests.Helpers.Player;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players.Inventory;

public class InventoryMoneyCalculationTests
{
    [Fact]
    public void Inventory_total_money_is_the_sum_of_coins_in_backpack()
    {
        //arrange
        var inventory = InventoryTestDataBuilder.Build();
        var backpack = ItemTestData.CreateBackpack();

        inventory.AddItem(backpack);

        var platinum = ItemTestData.CreateCoin(id: 1, amount: 50, multiplier: 100);
        var gold = ItemTestData.CreateCoin(id: 2, amount: 10, multiplier: 1);
        var crystal = ItemTestData.CreateCoin(id: 3, amount: 2, multiplier: 10_000);

        var bag = ItemTestData.CreateBackpack();

        bag.AddItem(crystal);
        backpack.AddItem(platinum);
        backpack.AddItem(gold);
        backpack.AddItem(bag);

        ICoinTypeStore coinTypeStore = new CoinTypeStore();
        coinTypeStore.Add(1, platinum.Metadata);
        coinTypeStore.Add(2, gold.Metadata);
        coinTypeStore.Add(3, crystal.Metadata);
        
        //assert
        inventory.GetTotalMoney(coinTypeStore).Should().Be(25010);
    }
}