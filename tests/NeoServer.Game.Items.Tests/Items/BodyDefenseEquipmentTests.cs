using System;
using FluentAssertions;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Items
{
    public class BodyDefenseEquipmentTests
    {
        [Fact]
        public void InspectionText_Armor_ReturnsText()
        {
            //arrange
            var sut = ItemTestData.CreateDefenseEquipmentItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.ArmorValue, 5)
            });

            //assert
            sut.InspectionText.Should().Be("(Arm: 5)");
        }

        [Fact]
        public void InspectionText_Shield_ReturnsText()
        {
            //arrange
            var sut = ItemTestData.CreateDefenseEquipmentItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Defense, 50)
            });

            //assert
            sut.InspectionText.Should().Be("(Def: 50)");
        }

        [Fact]
        public void Pickupable_ReturnsTrue()
        {
            //arrange
            var sut = ItemTestData.CreateDefenseEquipmentItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Defense, 50)
            });

            //assert
            sut.Pickupable.Should().BeTrue();
        }

        [Fact]
        public void IsApplicable_Null_ReturnsFalse()
        {
            //act
            var actual = BodyDefenseEquipment.IsApplicable(null);

            //assert
            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData("body")]
        [InlineData("legs")]
        [InlineData("head")]
        [InlineData("feet")]
        [InlineData("shield")]
        [InlineData("ring")]
        [InlineData("necklace")]
        public void IsApplicable_ReturnsTrue(string slot)
        {
            //arrange
            var sut = ItemTestData.CreateDefenseEquipmentItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.BodyPosition, slot)
            });
            //act
            var actual = BodyDefenseEquipment.IsApplicable(sut.Metadata);

            //assert
            actual.Should().BeTrue();
        }
        [Theory]
        [InlineData("backpack")]
        [InlineData("ammo")]
        [InlineData("two-handed")]
        [InlineData("weapon")]
        public void IsApplicable_ReturnsFalse(string slot)
        {
            //arrange
            var sut = ItemTestData.CreateDefenseEquipmentItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.BodyPosition, slot)
            });
            //act
            var actual = BodyDefenseEquipment.IsApplicable(sut.Metadata);

            //assert
            actual.Should().BeFalse();
        }
    }
}