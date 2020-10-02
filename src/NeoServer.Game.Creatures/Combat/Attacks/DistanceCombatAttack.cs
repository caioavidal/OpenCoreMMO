using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Enums.Item;
using NeoServer.Server.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Combat.Attacks
{
    public class DistanceAreaCombatAttack : DistanceCombatAttack, IDistanceAreaCombatAttack
    {
        public DistanceAreaCombatAttack(DamageType damageType, CombatAttackOption option) : base(damageType, option)
        {
        }

        public byte Radius => Option.Radius;
    }

    public class DistanceCombatAttack : CombatAttack, IDistanceCombatAttack
    {
        public DistanceCombatAttack(DamageType damageType, CombatAttackOption option) : base(damageType)
        {
            Option = option;
        }
        protected CombatAttackOption Option { get; }
        public byte Chance => Option.Chance;
        public byte Range => Option.Range;
        public virtual byte Target => Option.Target;
        public ShootType ShootType => Option.ShootType;

      


        public new ushort CalculateDamage(ushort attackPower, ushort minAttackPower)
        {
            attackPower = Option.Max;
            minAttackPower = Option.Min;

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
