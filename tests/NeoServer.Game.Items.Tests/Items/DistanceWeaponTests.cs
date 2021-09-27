using System;
using FluentAssertions;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Items
{
    public class DistanceWeaponTests
    {
        [Fact]
        public void InspectionText_NoAttributeFound_ReturnsText()
        {
            var sut = ItemTestData.CreateDistanceWeapon(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
            });

            //assert
            sut.InspectionText.Should().BeEmpty();
        }
        
        
        [Theory]
        [InlineData(6,7,3,"(Range: 6, Atk: +7, Hit% +3)")]
        [InlineData(6,0,3,"(Range: 6, Hit% +3)")]
        [InlineData(6,7,0,"(Range: 6, Atk: +7)")]
        [InlineData(6,0,0,"(Range: 6)")]
        [InlineData(0,7,3,"(Atk: +7, Hit% +3)")]
        [InlineData(0,7,0,"(Atk: +7)")]
        [InlineData(0,0,3,"(Hit% +3)")]
        [InlineData(0,0,0,"")]
        public void InspectionText_AttributeFound_ReturnsText(int range, int attack, int chance, string expected)
        {
            var sut = ItemTestData.CreateDistanceWeapon(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Range, range),
                (ItemAttribute.Attack,attack),
                (ItemAttribute.HitChance,chance)
            });

            //assert
            sut.InspectionText.Should().Be(expected);
        }
    }
}