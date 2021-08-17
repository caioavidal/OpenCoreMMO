using System;
using System.Collections.Generic;
using FluentAssertions;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.Creatures.Model.Players;
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
    }
}