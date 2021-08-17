using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.DataStore;
using NeoServer.Game.Tests.Helpers;
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
        public void ChangeChaseMode_ChangesChaseMode()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer();
            sut.ChangeChaseMode(ChaseMode.Stand);

            sut.ChaseMode.Should().Be(ChaseMode.Stand);
            sut.ChangeChaseMode(ChaseMode.Follow);

            sut.ChaseMode.Should().Be(ChaseMode.Follow);
        }

        [Fact]
        public void ChangeChaseMode_Follow_InvokeFollow()
        {

            var sut = PlayerTestDataBuilder.BuildPlayer();
            var enemy = PlayerTestDataBuilder.BuildPlayer();
            GameToolStore.PathFinder = new Mock<IPathFinder>().Object;

            var called = false;
            sut.OnStartedFollowing += (_, _, _) =>
            {
                called = true;
            };

            sut.ChangeChaseMode(ChaseMode.Stand);

            sut.SetAttackTarget(enemy);

            called.Should().BeFalse();

            sut.ChangeChaseMode(ChaseMode.Follow);

            called.Should().BeTrue();
        }
        [Fact]
        public void ChangeChaseMode_Stand_SetsFollowingToFalse()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer();
            var enemy = PlayerTestDataBuilder.BuildPlayer();
            GameToolStore.PathFinder = new Mock<IPathFinder>().Object;

            sut.SetAttackTarget(enemy);

            sut.ChangeChaseMode(ChaseMode.Follow);
            
            sut.IsFollowing.Should().BeTrue();

            sut.ChangeChaseMode(ChaseMode.Stand);

            sut.IsFollowing.Should().BeFalse();
        }

        [Fact]
        public void ChangeSecureMode_ChangesSecureMode()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer();

            sut.ChangeSecureMode(0);
            sut.SecureMode.Should().Be(0);
            sut.ChangeSecureMode(1);
            sut.SecureMode.Should().Be(1);
        }
    }
}