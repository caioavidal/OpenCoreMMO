using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Enums.Item;
using NeoServer.Server.Helpers;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures.Model.Monsters
{

    public class CombatAttack : ICombatAttack
    {
        public CombatAttack(DamageType damageType)
        {
            DamageType = damageType;
        }

        public DamageType DamageType { get; private set; }

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
    }

    public class MeleeCombatAttack : CombatAttack, IMeleeCombatAttack
    {
        public MeleeCombatAttack(byte attack, byte skill) : base(DamageType.Melee)
        {
            Attack = attack;
            Skill = skill;
        }
        public byte Attack { get; }
        public byte Skill { get; }

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
    }
}
