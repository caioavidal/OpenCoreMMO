using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Combat.Attacks
{
    //public class DistanceAreaCombatAttack : DistanceCombatAttack
    //{
    //    public byte Radius { get; set; }
    //}

    public class DistanceCombatAttack : CombatAttack, IDistanceCombatAttack
    {
        public DistanceCombatAttack(DamageType damageType, CombatAttackOption option) : base(damageType)
        {
            _option = option;
        }
        private CombatAttackOption _option;
        public byte Chance => _option.Chance;
        public byte Range => _option.Range;
        public virtual byte Target => _option.Target;
        public ShootType ShootType => _option.ShootType;


        //public new ushort CalculateDamage(ushort attackPower, ushort minAttackPower)
        //{
        //    var diff = attackPower - minAttackPower;
        //    var gaussian = _random.Next(0.5f, 0.25f);

        //    double increment;
        //    if (gaussian < 0.0)
        //    {
        //        increment = diff / 2;
        //    }
        //    else if (gaussian > 1.0)
        //    {
        //        increment = (diff + 1) / 2;
        //    }
        //    else
        //    {
        //        increment = Math.Round(gaussian * diff);
        //    }
        //    return (ushort)(minAttackPower + increment);
        //}
    }
}
