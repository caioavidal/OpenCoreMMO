using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Creature.Model;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
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
        public void CanMoveThing_Given_Distance_Bigger_Than_11_Returns_False(int toX, int toY, bool expected)
        {
            var sut = new Player(123456, "PlayerA", ChaseMode.Stand, 100, healthPoints: 100, maxHealthPoints: 100, vocation: VocationType.Knight, Gender.Male, online: true, mana: 30, maxMana: 30, fightMode: FightMode.Attack,
                soulPoints: 100, maxSoulPoints: 100, skills: new Dictionary<SkillType, ISkill>
                {
                    { SkillType.Axe, new Skill(SkillType.Axe, 100,1,1,100,100,1) }

                }, staminaMinutes: 300, outfit: new Outfit(), inventory: new Dictionary<Slot, Tuple<IPickupable, ushort>>(), speed: 300, new Location(100, 100, 7));


            Assert.Equal(expected, sut.CanMoveThing(new Location(toX, toY, 7)));
        }
    }
}
