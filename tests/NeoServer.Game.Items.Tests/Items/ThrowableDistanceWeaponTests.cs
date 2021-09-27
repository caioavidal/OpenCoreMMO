using System;
using FluentAssertions;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Items
{
    public class ThrowableDistanceWeaponTests
    {
        [Theory]
        [InlineData(6,7,10,3,"(Range: 6, Atk: 7, Def: 10, Hit% +3)")]
        [InlineData(0,10,1,0, "(Atk: 10, Def: 1)")]
        [InlineData(0,0,0,0, "(Atk: 0, Def: 0)")]
        public void InspectionText_AttributeFound_ReturnsText(int range, int attack, int defense,int chance, string expected)
        {
            var sut = ItemTestData.CreateThrowableDistanceItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Range, range),
                (ItemAttribute.Attack,attack),
                (ItemAttribute.Defense,defense),
                (ItemAttribute.HitChance,chance)
            });

            //assert
            sut.InspectionText.Should().Be(expected);
        }

        [Theory]
        [InlineData(ItemAttribute.ElementFire, 5, "(Atk: 6 + 5 fire, Def: 7)")]
        [InlineData(ItemAttribute.ElementEarth, 10, "(Atk: 6 + 10 earth, Def: 7)")]
        [InlineData(ItemAttribute.ElementEnergy, 1, "(Atk: 6 + 1 energy, Def: 7)")]
        [InlineData(ItemAttribute.ElementIce, 23, "(Atk: 6 + 23 ice, Def: 7)")]
        [InlineData(ItemAttribute.ElementIce, 0, "(Atk: 6, Def: 7)")]
        public void InspectionText_HasElementalDamage_ReturnsText(ItemAttribute itemAttribute, int elementalDamage,
            string expected)
        {
            var sut = ItemTestData.CreateWeaponItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Attack, 6),
                (ItemAttribute.Defense, 7),
                (itemAttribute, elementalDamage),
            });

            //assert
            sut.InspectionText.Should().Be(expected);
        }
    }
}