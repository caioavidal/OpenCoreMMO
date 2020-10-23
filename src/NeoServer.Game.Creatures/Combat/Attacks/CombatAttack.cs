using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Enums.Item;
using NeoServer.Server.Helpers;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures.Model.Monsters
{

    public class CombatAttack : ICombatAttack
    {
        public CombatAttack(DamageType damageType, CombatAttackOption option)
        {
            option.DamageType = damageType;
            Option = option;
        }

        public CombatAttackOption Option { get; }

        public virtual void BuildAttack(ICreature actor, ICreature enemy)
        {

        }
        
        public virtual ushort CalculateDamage(ushort attackPower, ushort minAttackPower)
        {
            var diff = attackPower - minAttackPower;
            var gaussian = GaussianRandom.Random.Next(0.5f, 0.25f);

            double increment;
            if (gaussian < 0.0)
            {
                increment = diff / 2;
            }
            else if (gaussian > 1.0)
            {
                increment = (diff + 1) / 2;
            }
            else
            {
                increment = Math.Round(gaussian * diff);
            }
            return (ushort)(minAttackPower + increment);
        }

        public virtual void CauseDamage(ICreature actor, ICreature enemy)
        {
            enemy.ReceiveAttack(actor, this, CalculateDamage(actor.AttackPower, actor.MinimumAttackPower));
        }
    }

  
}
