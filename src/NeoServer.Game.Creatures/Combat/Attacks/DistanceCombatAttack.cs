using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Enums.Item;
using NeoServer.Server.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Combat.Attacks
{
   

    public class DistanceCombatAttack : CombatAttack, IDistanceCombatAttack
    {
        public DistanceCombatAttack(DamageType damageType, CombatAttackOption option) : base(damageType, option)
        {
        }
        public byte Chance => Option.Chance;
        public byte Range => Option.Range;
        public bool HasTarget => true;
        public override void CauseDamage(ICombatActor actor, ICombatActor enemy)
        {
            base.CauseDamage(actor, enemy);
        }
    }
}
