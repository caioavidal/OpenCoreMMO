using System;
using FluentAssertions;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Items
{
    public class MeleeWeaponTests
    {
        [Theory]
        [InlineData(6, 7, 10, "(Atk: 6, Def: 7 +10)")]
        [InlineData(10, 1, 0, "(Atk: 10, Def: 1)")]
        [InlineData(25, 0, 5, "(Atk: 25, Def: 0 +5)")]
        [InlineData(0, 10, 5, "(Atk: 0, Def: 10 +5)")]
        [InlineData(0, 0, 5, "(Atk: 0, Def: 0 +5)")]
        [InlineData(0, 0, 0, "(Atk: 0, Def: 0)")]
        public void InspectionText_ReturnsText(int attack, int defense, int extraDef, string expected)
        {
            var sut = ItemTestData.CreateWeaponItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Attack, attack),
                (ItemAttribute.Defense, defense),
                (ItemAttribute.ExtraDefense, extraDef)
            });

            //assert
            sut.InspectionText.Should().Be(expected);
        }

        [Theory]
        [InlineData(ItemAttribute.ElementFire, 5, "(Atk: 6 + 5 fire, Def: 7 +10)")]
        [InlineData(ItemAttribute.ElementEarth, 10, "(Atk: 6 + 10 earth, Def: 7 +10)")]
        [InlineData(ItemAttribute.ElementEnergy, 1, "(Atk: 6 + 1 energy, Def: 7 +10)")]
        [InlineData(ItemAttribute.ElementIce, 23, "(Atk: 6 + 23 ice, Def: 7 +10)")]
        [InlineData(ItemAttribute.ElementIce, 0, "(Atk: 6, Def: 7 +10)")]
        public void InspectionText_HasElementalDamage_ReturnsText(ItemAttribute itemAttribute, int elementalDamage,
            string expected)
        {
            var sut = ItemTestData.CreateWeaponItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Attack, 6),
                (ItemAttribute.Defense, 7),
                (ItemAttribute.ExtraDefense, 10),
                (itemAttribute, elementalDamage),
            });

            //assert
            sut.InspectionText.Should().Be(expected);
        }
    }
}