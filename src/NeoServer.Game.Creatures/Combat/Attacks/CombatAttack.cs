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
            var gaussian = GaussianRandom.Randon.Next(0.5f, 0.25f);

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
        //public ushort Interval { get; set; }
        //public ushort Skill { get; set; }
        //public ushort Attack { get; set; }
        //public Tuple<SkillType, byte> Skill { get; set; }
        //public byte Chance { get; set; }
        //public byte Range { get; set; }
        //public byte Radius { get; set; }
        //public byte Target { get; set; }
        //public Tuple<short,short> MinMax { get; set; }

        //public IDictionary<string, string> Attributes { get; set; }

        //public ShootType ShootType
        //{
        //    get
        //    {
        //        if (Attributes.TryGetValue("shootEffect", out var shootEffect))
        //        {
        //            return shootEffect switch
        //            {
        //                "spear" => ShootType.Spear,
        //                _ => ShootType.None
        //            };
        //        }
        //        return ShootType.None;

        //    }
        //}

        //public ushort MaxAttack { get; internal set; }
        //public ushort MinAttack { get; internal set; }
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
    }
}
