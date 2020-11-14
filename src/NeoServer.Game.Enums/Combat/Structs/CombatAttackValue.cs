using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Enums.Combat.Structs
{
    public ref struct CombatAttackValue
    {
        public CombatAttackValue(ushort minDamage, ushort maxDamage, DamageType damageType) : this()
        {
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            DamageType = damageType;
        }

        public CombatAttackValue(ushort minDamage, ushort maxDamage, byte range, byte hitChance, DamageType damageType) : this()
        {
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            Range = range;
            HitChance = hitChance;
            DamageType = damageType;

        }

        public byte Range { get; set; }
        public ushort MinDamage { get; set; }
        public ushort MaxDamage { get; set; }
        public DamageType DamageType { get; set; }
        public byte HitChance { get; set; }
    }

    public ref struct CombatAttackType
    {
        public CombatAttackType(ShootType shootType) : this()
        {
            ShootType = shootType;
        }

        public CombatAttackType(DamageType damageType) : this()
        {
            DamageType = damageType;
        }
        public bool Missed { get; set; }
        public ShootType ShootType { get; set; }
        public DamageType DamageType { get; set; }

    }
}
