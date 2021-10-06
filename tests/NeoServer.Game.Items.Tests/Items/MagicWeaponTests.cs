using System;
using System.Collections.Generic;
using FluentAssertions;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Items
{
    public class MagicWeaponTests
    {
        [Fact]
        public void InspectionText_NoAttributeFound_ReturnsText()
        {
            var sut = ItemTestData.CreateMagicWeapon(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
            });
        
            //assert
            sut.InspectionText.Should().BeEmpty();
        }
        #region CanBeDressed Tests

        [InlineData(2,  1)]
        [InlineData(2,  3)]
        [Theory]
        public void CanBeDressed_PlayerHasNotRequiredVocation_ReturnsFalse(int playerVocation,
            int requiredVocation)
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer(vocation: (byte)playerVocation);
            var sut = ItemTestData.CreateMagicWeapon(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.BodyPosition, "body"),
            });
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.Vocation, new[]{(byte)requiredVocation});

            //act
            var actual = sut.CanBeDressed(player);

            //assert
            actual.Should().BeFalse();
        }
        [InlineData(2, 1, 2, 10)]
        [InlineData(2, 8, 2, 10)]
        [InlineData(5, 0, 5, 0)]
        [InlineData(5, 1, 5, 1)]
        [Theory]
        public void CanBeDressed_PlayerHasVocationAndNoMinimumLevel_ReturnsTrue(int playerVocation, int playerLevel, int requiredVocation, int minLevel)
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer(vocation: (byte)playerVocation,
                skills: new Dictionary<SkillType, ISkill>()
                {
                    [SkillType.Level] = new Skill(SkillType.Level, 1, (ushort)playerLevel, 0)
                });
            var sut = ItemTestData.CreateMagicWeapon(id: 1, attributes: new (ItemAttribute, IConvertible)[]
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
        
        [Fact]
        public void CanBeDressed_ItemHasNoRequiredVocation_ReturnsTrue()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer(vocation: (byte)1);
            var sut = ItemTestData.CreateMagicWeapon(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.BodyPosition, "body"),
            });

            //act
            var actual = sut.CanBeDressed(player);

            //assert
            actual.Should().BeTrue();
        }

        #endregion
    }
}