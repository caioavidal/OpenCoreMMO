using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Creature.Model;
using NeoServer.Game.Tests;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using Xunit;

namespace NeoServer.Game.Creatures.Tests
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
            var sut = new Player(1, "PlayerA", ChaseMode.Stand, 100, healthPoints: 100, maxHealthPoints: 100, vocation: 1, Gender.Male, online: true, mana: 30, maxMana: 30, fightMode: FightMode.Attack,
                soulPoints: 100, soulMax: 100, skills: new Dictionary<SkillType, ISkill>
                {
                    { SkillType.Axe, new Skill(SkillType.Axe, 1.1f,10,0)  }

                }, staminaMinutes: 300, outfit: new Outfit(), inventory: new Dictionary<Slot, Tuple<IPickupable, ushort>>(), speed: 300, new Location(100, 100, 7),
                pathAccess: new CreaturePathAccess(new World.Map.PathFinder(null).Find, null));

            Assert.Equal(expected, sut.CanMoveThing(new Location(toX, toY, 7)));
        }

        [Fact]
        public void OnDamage_When_Receives_Melee_Attack_Reduce_Health()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp:100) as Player;
            var enemy = PlayerTestDataBuilder.BuildPlayer() as Player;
            sut.OnDamage(enemy, new(5, Common.Item.DamageType.Melee));

            Assert.Equal((uint)95, sut.HealthPoints);
        }
        [Fact]
        public void OnDamage_When_Receives_Mana_Attack_Reduce_Mana()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(mana:30) as Player;
            var enemy = PlayerTestDataBuilder.BuildPlayer() as Player;
            sut.OnDamage(enemy, new(5, Common.Item.DamageType.ManaDrain));

            Assert.Equal((uint)25, sut.Mana);
        }
    }
}
