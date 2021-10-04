using System;
using FluentAssertions;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Items
{
    public class AmmoTests
    {
        [Theory]
        [InlineData(6, "(Atk: 6)")]
        [InlineData(10, "(Atk: 10)")]
        [InlineData(1, "(Atk: 1)")]
        public void InspectionText_ReturnsText(int attack, string expected)
        {
            var sut = ItemTestData.CreateAmmoItem(id: 1, amount:10, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Attack, attack)
            });

            //assert
            sut.InspectionText.Should().Be(expected);
        }

        [Theory]
        [InlineData(ItemAttribute.ElementFire, 5, "(Atk: 6 + 5 fire)")]
        [InlineData(ItemAttribute.ElementEarth, 10, "(Atk: 6 + 10 earth)")]
        [InlineData(ItemAttribute.ElementEnergy, 1, "(Atk: 6 + 1 energy)")]
        [InlineData(ItemAttribute.ElementIce, 23, "(Atk: 6 + 23 ice)")]
        [InlineData(ItemAttribute.ElementIce, 0, "(Atk: 6)")]
        public void InspectionText_HasElementalDamage_ReturnsText(ItemAttribute itemAttribute, int elementalDamage,
            string expected)
        {
            var sut = ItemTestData.CreateAmmoItem(id: 1, amount:10, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Attack, 6),
                (itemAttribute, elementalDamage),
            });

            //assert
            sut.InspectionText.Should().Be(expected);
        }
    }
}