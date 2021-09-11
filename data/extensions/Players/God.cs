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

namespace NeoServer.Extensions.Players
{
    public class God : Tutor
    {
        public God(uint id, string characterName, byte vocation, Gender gender, bool online,
            IDictionary<SkillType, ISkill> skills, IOutfit outfit,
            IDictionary<Slot, Tuple<IPickupable, ushort>> inventory, ushort speed, Location location) :
            base(id, characterName, vocation, gender, online, skills, outfit, inventory, speed, location)
        {
            SetFlags(PlayerFlag.CanSeeInvisibility, PlayerFlag.SpecialVip);
        }

        public override bool CanSeeInvisible => FlagIsEnabled(PlayerFlag.CanSeeInvisibility);
        public override bool CannotLogout => false;
        public override bool CanBeSeen => false;

        public override void GainExperience(uint exp)
        {
        } //tutor do not gain experience

        public override bool ReceiveAttack(IThing enemy, CombatDamage damage)
        {
            return false;
        }

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