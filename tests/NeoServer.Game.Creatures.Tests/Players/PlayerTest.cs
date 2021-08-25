using FluentAssertions;
using Moq;
using NeoServer.Extensions.Runes;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Items.Items.UsableItems.Runes;
using NeoServer.Game.Tests.Helpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players
{
    public class PlayerTest
    {
        [Theory]
        [InlineData(100, 111, true)]
        [InlineData(100, 112, false)]
        [InlineData(105, 106, true)]
        [InlineData(95, 94, true)]
        [InlineData(94, 94, false)]
        public void CanMoveThing_Given_Distance_Bigger_Than_11_Returns_False(ushort toX, ushort toY, bool expected)
        {
            var sut = new Player(1, "PlayerA", ChaseMode.Stand, 100, 100, 100, 1, Gender.Male, true, 30, 30,
                FightMode.Attack,
                100, 100, new Dictionary<SkillType, ISkill>
                {
                    {SkillType.Axe, new Skill(SkillType.Axe, 1.1f, 10)}
                }, 300, new Outfit(), new Dictionary<Slot, Tuple<IPickupable, ushort>>(), 300,
                new Location(100, 100, 7));

            Assert.Equal(expected, sut.CanMoveThing(new Location(toX, toY, 7)));
        }

        [Fact]
        public void OnDamage_When_Receives_Melee_Attack_Reduce_Health()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100) as Player;
            var enemy = PlayerTestDataBuilder.BuildPlayer() as Player;
            sut.OnDamage(enemy, new CombatDamage(5, DamageType.Melee));

            Assert.Equal((uint) 95, sut.HealthPoints);
        }

        [Fact]
        public void OnDamage_When_Receives_Mana_Attack_Reduce_Mana()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(mana: 30) as Player;
            var enemy = PlayerTestDataBuilder.BuildPlayer() as Player;
            sut.OnDamage(enemy, new CombatDamage(5, DamageType.ManaDrain));

            Assert.Equal((uint) 25, sut.Mana);
        }

        [Fact]
        public void FlagIsEnabled_Enabled_ReturnsTrue()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer();
            sut.SetFlag(PlayerFlag.CanBeSeen);
            var result = sut.FlagIsEnabled(PlayerFlag.CanBeSeen);

            result.Should().BeTrue();
        }
        [Fact]
        public void FlagIsEnabled_Disabled_ReturnsTrue()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer();
            var result = sut.FlagIsEnabled(PlayerFlag.CanBeSeen);

            result.Should().BeFalse();
        }

        [Fact]
        public void SetFightMode_ChangesMode()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer();
            sut.ChangeFightMode(FightMode.Defense);

            sut.FightMode.Should().Be(FightMode.Defense);
            sut.ChangeFightMode(FightMode.Attack);

            sut.FightMode.Should().Be(FightMode.Attack);
        }

        [Fact]
        public void PlayerDoesNotGainSkillsWhenUsingAnAttackRune()
        {
            var player = PlayerTestDataBuilder.BuildPlayer();
            var targetPlayer = PlayerTestDataBuilder.BuildPlayer();

            var itemAttirbuteListMock = new Mock<IItemAttributeList>();

            itemAttirbuteListMock
                .Setup(x => x.GetAttribute<bool>(It.IsAny<ItemAttribute>()))
                .Returns(true);

            itemAttirbuteListMock
                .Setup(x => x.GetAttributeArray(It.IsAny<string>()))
                .Returns(new dynamic[] { "0.0", "0.0" });

            var itemTypeMock = new Mock<IItemType>();
            itemTypeMock
                .Setup(x => x.Attributes)
                .Returns(itemAttirbuteListMock.Object);

            var rune = new AttackRune(itemTypeMock.Object, new Location(100, 100, 7), (byte)10);

            var before = new Dictionary<SkillType, byte>()
            {
                { SkillType.Axe, player.GetSkillTries(SkillType.Axe) },
                { SkillType.Club, player.GetSkillTries(SkillType.Club) },
                { SkillType.Distance, player.GetSkillTries(SkillType.Distance) },
                { SkillType.Fishing, player.GetSkillTries(SkillType.Fishing) },
                { SkillType.Fist, player.GetSkillTries(SkillType.Fist) },
                { SkillType.Level, player.GetSkillTries(SkillType.Level) },
                { SkillType.Magic, player.GetSkillTries(SkillType.Magic) },
                { SkillType.Shielding, player.GetSkillTries(SkillType.Shielding) },
                { SkillType.Speed, player.GetSkillTries(SkillType.Speed) },
                { SkillType.Sword, player.GetSkillTries(SkillType.Sword) },
            };

            var result = rune.Use(player, targetPlayer, out var attackType);
            Assert.True(result);

            var after = new Dictionary<SkillType, byte>()
            {
                { SkillType.Axe, player.GetSkillTries(SkillType.Axe) },
                { SkillType.Club, player.GetSkillTries(SkillType.Club) },
                { SkillType.Distance, player.GetSkillTries(SkillType.Distance) },
                { SkillType.Fishing, player.GetSkillTries(SkillType.Fishing) },
                { SkillType.Fist, player.GetSkillTries(SkillType.Fist) },
                { SkillType.Level, player.GetSkillTries(SkillType.Level) },
                { SkillType.Magic, player.GetSkillTries(SkillType.Magic) },
                { SkillType.Shielding, player.GetSkillTries(SkillType.Shielding) },
                { SkillType.Speed, player.GetSkillTries(SkillType.Speed) },
                { SkillType.Sword, player.GetSkillTries(SkillType.Sword) },
            };

            foreach (var skillType in before.Keys)
            {
                Assert.Equal(before[skillType], after[skillType]);
            }
        }

        [Fact]
        public void PlayerDoesNotGainSkillsWhenUsingAHealingRune()
        {
            var player = PlayerTestDataBuilder.BuildPlayer();
            var targetPlayer = PlayerTestDataBuilder.BuildPlayer();

            var itemAttirbuteListMock = new Mock<IItemAttributeList>();

            itemAttirbuteListMock
                .Setup(x => x.GetAttribute<bool>(It.IsAny<ItemAttribute>()))
                .Returns(true);

            itemAttirbuteListMock
                .Setup(x => x.GetAttributeArray(It.IsAny<string>()))
                .Returns(new dynamic[] { "0.0", "0.0" });

            var itemTypeMock = new Mock<IItemType>();
            itemTypeMock
                .Setup(x => x.Attributes)
                .Returns(itemAttirbuteListMock.Object);

            var rune = new HealingRune(itemTypeMock.Object, new Location(100, 100, 7), new Dictionary<ItemAttribute, IConvertible>());

            var before = new Dictionary<SkillType, byte>()
            {
                { SkillType.Axe, player.GetSkillTries(SkillType.Axe) },
                { SkillType.Club, player.GetSkillTries(SkillType.Club) },
                { SkillType.Distance, player.GetSkillTries(SkillType.Distance) },
                { SkillType.Fishing, player.GetSkillTries(SkillType.Fishing) },
                { SkillType.Fist, player.GetSkillTries(SkillType.Fist) },
                { SkillType.Level, player.GetSkillTries(SkillType.Level) },
                { SkillType.Magic, player.GetSkillTries(SkillType.Magic) },
                { SkillType.Shielding, player.GetSkillTries(SkillType.Shielding) },
                { SkillType.Speed, player.GetSkillTries(SkillType.Speed) },
                { SkillType.Sword, player.GetSkillTries(SkillType.Sword) },
            };

            rune.Use(player, targetPlayer);

            var after = new Dictionary<SkillType, byte>()
            {
                { SkillType.Axe, player.GetSkillTries(SkillType.Axe) },
                { SkillType.Club, player.GetSkillTries(SkillType.Club) },
                { SkillType.Distance, player.GetSkillTries(SkillType.Distance) },
                { SkillType.Fishing, player.GetSkillTries(SkillType.Fishing) },
                { SkillType.Fist, player.GetSkillTries(SkillType.Fist) },
                { SkillType.Level, player.GetSkillTries(SkillType.Level) },
                { SkillType.Magic, player.GetSkillTries(SkillType.Magic) },
                { SkillType.Shielding, player.GetSkillTries(SkillType.Shielding) },
                { SkillType.Speed, player.GetSkillTries(SkillType.Speed) },
                { SkillType.Sword, player.GetSkillTries(SkillType.Sword) },
            };

            foreach (var skillType in before.Keys)
            {
                Assert.Equal(before[skillType], after[skillType]);
            }
        }
    }
}