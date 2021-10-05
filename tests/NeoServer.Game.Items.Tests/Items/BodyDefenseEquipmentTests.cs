﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Creatures.Model.Players;
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

        #region CanBeDressed Tests

           [InlineData(2, 9, 1, 10)]
        [InlineData(2, 9, 2, 10)]
        [InlineData(2, 10, 1, 10)]
        [Theory]
        public void CanBeDressed_PlayerHasNoLevelOrNoVocation_ReturnsFalse(int playerVocation, int playerLevel,
            int requiredVocation, int minLevel)
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer(vocation: (byte)playerVocation,
                skills: new Dictionary<SkillType, ISkill>()
                {
                    [SkillType.Level] = new Skill(SkillType.Level, 1, (ushort)playerLevel, 0)
                });
            var sut = ItemTestData.CreateDefenseEquipmentItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.BodyPosition, "body"),
                (ItemAttribute.MinimumLevel, minLevel),
            });
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.Vocation, new[]{(byte)requiredVocation});

            //act
            var actual = sut.CanBeDressed(player);

            //assert
            actual.Should().BeFalse();
        }
        
        [InlineData(2, 10, 2, 10)]
        [InlineData(2, 11, 2, 10)]
        [InlineData(5, 0, 5, 0)]
        [Theory]
        public void CanBeDressed_PlayerHasBothVocationAndLevel_ReturnsTrue(int playerVocation, int playerLevel, int requiredVocation, int minLevel)
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer(vocation: (byte)playerVocation,
                skills: new Dictionary<SkillType, ISkill>()
                {
                    [SkillType.Level] = new Skill(SkillType.Level, 1, (ushort)playerLevel, 0)
                });
            var sut = ItemTestData.CreateDefenseEquipmentItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.BodyPosition, "body"),
                (ItemAttribute.MinimumLevel, minLevel),
            });
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.Vocation, new[]{(byte)requiredVocation});

            //act
            var actual = sut.CanBeDressed(player);

            //assert
            actual.Should().BeTrue();
        }
        [InlineData(10, 10)]
        [InlineData(2, 1)]
        [InlineData(0, 0)]
        [Theory]
        public void CanBeDressed_ItemDoesNotRequireVocationButLevel_ReturnsTrue(int playerLevel, int minLevel)
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer(vocation: (byte)1,
                skills: new Dictionary<SkillType, ISkill>()
                {
                    [SkillType.Level] = new Skill(SkillType.Level, 1, (ushort)playerLevel, 0)
                });
            var sut = ItemTestData.CreateDefenseEquipmentItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.BodyPosition, "body"),
                (ItemAttribute.MinimumLevel, minLevel),
            });

            //act
            var actual = sut.CanBeDressed(player);

            //assert
            actual.Should().BeTrue();
        }
        
        [InlineData(10, 10)]
        [InlineData(2, 2)]
        [InlineData(0, 0)]
        [Theory]
        public void CanBeDressed_ItemRequiresVocationButNoLevel_ReturnsTrue(int playerVocation, int requiredVocation)
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer(vocation: (byte)playerVocation,
                skills: new Dictionary<SkillType, ISkill>()
                {
                    [SkillType.Level] = new Skill(SkillType.Level, 1, (ushort)1, 0)
                });
            var sut = ItemTestData.CreateDefenseEquipmentItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.BodyPosition, "body"),
            });
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.Vocation, new[]{(byte)requiredVocation});

            //act
            var actual = sut.CanBeDressed(player);

            //assert
            actual.Should().BeTrue();
        }

        #endregion
     
    }
}
