using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Scripts.Players
{
    public class Tutor : Player
    {
        public Tutor(uint id, string characterName, Gender gender, bool online, IDictionary<SkillType, ISkill> skills, IOutfit outfit, IDictionary<Slot, Tuple<IPickupable, ushort>> inventory, ushort speed, Location location, IPathAccess pathAccess) :
            base(id, characterName, ChaseMode.Follow, uint.MaxValue, ushort.MaxValue, ushort.MaxValue, VocationType.None, gender, online, ushort.MaxValue, ushort.MaxValue, FightMode.Balanced, 100, 100, skills, 60, outfit, inventory, speed, location, pathAccess)
        {

        }

        public override void GainExperience(uint exp) { } //tutor do not gain experience
        public override bool ReceiveAttack(ICombatActor enemy, CombatDamage damage)
        {
            return false;
        }
        public override void OnDamage(ICombatActor enemy, CombatDamage damage) { }
    }
}
