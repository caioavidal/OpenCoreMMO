using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.Creatures.Model.Players;

namespace NeoServer.Game.Tests.Helpers
{
    public class PlayerTestDataBuilder
    {
        public static IPlayer BuildPlayer(uint id = 1, uint capacity = 100, ushort hp = 100, ushort mana = 30, ushort speed = 200,
            Dictionary<Slot, Tuple<IPickupable, ushort>> inventory = null, Dictionary<SkillType, ISkill> skills = null)
        {
            inventory = inventory ?? new Dictionary<Slot, Tuple<IPickupable, ushort>>();
            var sut = new Player(id, "PlayerA", ChaseMode.Stand, capacity, hp, 100, 1, Gender.Male, true, mana, 30,
                FightMode.Attack,
                100, 100,
                skills ?? new Dictionary<SkillType, ISkill> {{SkillType.Level, new Skill(SkillType.Level, 1, 10, 1)}},
                300, new Outfit(), inventory, speed, new Location(100, 100, 7));
            return sut;
        }
    }
}