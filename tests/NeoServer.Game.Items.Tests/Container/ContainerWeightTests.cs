using FluentAssertions;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.Tests.Helpers.Player;
using Xunit;

namespace NeoServer.Game.Items.Tests.Container;

public class ContainerWeightTests
{
    [Fact]
    public void Container_weight_increases_when_items_are_added_to_it()
    {
        //arrange
        var backpack = ItemTestData.CreateBackpack(weight: 18);

        var weapon = ItemTestData.CreateWeaponItem(id: 2, weight: 40);
        var fiveArrows = ItemTestData.CreateCumulativeItem(id: 3, amount: 5, weight: 0.70f);
        var twoArrows = ItemTestData.CreateCumulativeItem(id: 3, amount: 2, weight: 0.70f);

        //assert
        backpack.Weight.Should().Be(18);

        //act
        backpack.AddItem(weapon);
        
        //assert
        backpack.Weight.Should().Be(58);
        
        //act
        backpack.AddItem(fiveArrows);
        
        //assert
        backpack.Weight.Should().Be(61.5f);
        
        //act
        backpack.AddItem(twoArrows);
        
        //assert
        backpack.Weight.Should().Be(62.9f);
    }
}