using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using System;
using System.Collections.Generic;

namespace NeoServer.Scripts.Players
{
    public class God : Tutor
    {
        public God(uint id, string characterName, byte vocation, Gender gender, bool online, IDictionary<SkillType, ISkill> skills, IOutfit outfit, IDictionary<Slot, Tuple<IPickupable, ushort>> inventory, ushort speed, Location location, IPathAccess pathAccess) : 
            base(id, characterName, vocation, gender, online, skills, outfit, inventory, speed, location, pathAccess)
        {
        }
        public override bool CanSeeInvisible => true;
        public override bool CanBeSeen => false;
        public override void GainExperience(uint exp) { } //tutor do not gain experience
        public override bool ReceiveAttack(IThing enemy, CombatDamage damage)
        {
            return false;
        }
        public override void OnDamage(IThing enemy, CombatDamage damage) { }
        public override void OnMoved(IDynamicTile fromTile, IDynamicTile toTile, ICylinderSpectator[] spectators) { }
        public override void OnCreatureAppear(Location location, ICylinderSpectator[] spectators) { }
    }
}
