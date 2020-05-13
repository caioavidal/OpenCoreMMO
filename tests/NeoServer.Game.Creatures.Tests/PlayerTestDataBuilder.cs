using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Creature.Model;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Tests
{
    internal class PlayerTestDataBuilder
    {
        public static IPlayer BuildPlayer()
        {
            var sut = new Player(123456, "PlayerA", ChaseMode.Stand, 100, healthPoints: 100, maxHealthPoints: 100, vocation: VocationType.Knight, Gender.Male, online: true, mana: 30, maxMana: 30, fightMode: FightMode.Attack,
              soulPoints: 100, maxSoulPoints: 100, skills: new Dictionary<SkillType, ISkill>
              {
                    { SkillType.Axe, new Skill(SkillType.Axe, 100,1,1,100,100,1) }

              }, staminaMinutes: 300, outfit: new Outfit(), inventory: new Dictionary<Slot, Tuple<IItem, ushort>>(), speed: 300, new Location(100, 100, 7));
            return sut;
        }
    }
}
