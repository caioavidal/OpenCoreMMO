using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Creature.Model;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures.Tests
{
    internal class PlayerTestDataBuilder
    {
        public static IPlayer BuildPlayer(uint capacity = 100)
        {
            var sut = new Player(1,"PlayerA", ChaseMode.Stand, capacity: capacity, healthPoints: 100, maxHealthPoints: 100, vocation: VocationType.Knight, Gender.Male, online: true, mana: 30, maxMana: 30, fightMode: FightMode.Attack,
              soulPoints: 100, skills: new Dictionary<SkillType, ISkill>
              {
                    { SkillType.Axe, new Skill(SkillType.Axe, 1.1f,10,0)  }

              }, staminaMinutes: 300, outfit: new Outfit(), inventory: new Dictionary<Slot, Tuple<IPickupable, ushort>>(), speed: 300, new Location(100, 100, 7), new World.Map.PathFinder(null).Find);
            return sut;
        }
    }
}
