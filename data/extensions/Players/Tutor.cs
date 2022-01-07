using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Model.Players;

namespace NeoServer.Extensions.Players
{
    public class Tutor : Player
    {
        public Tutor(uint id, string characterName, IVocation vocation, Gender gender, bool online,
            IDictionary<SkillType, ISkill> skills, IOutfit outfit, ushort speed, Location location, 
            IPathFinder pathFinder,
            IWalkToMechanism walkToMechanism,
            Func<Location, Location, bool> isSightClearFunc) :
            base(id, characterName, ChaseMode.Follow, ushort.MaxValue, ushort.MaxValue, ushort.MaxValue, vocation,
                gender, online, ushort.MaxValue, ushort.MaxValue, FightMode.Balanced, 100, 100, skills, 60, outfit,
                speed, location, pathFinder, walkToMechanism, isSightClearFunc)
        {
        }

        public override bool CanBeAttacked => false;

        public override void GainExperience(uint exp)
        {
        } //tutor do not gain experience

        public override void OnDamage(IThing enemy, CombatDamage damage)
        {
        }

        public override void OnMoved(IDynamicTile fromTile, IDynamicTile toTile, ICylinderSpectator[] spectators)
        {
        }

        public override void OnCreatureAppear(Location location, ICylinderSpectator[] spectators)
        {
        }
    }
}