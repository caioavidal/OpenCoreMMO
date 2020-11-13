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

        public CombatAttackValue(ushort minDamage, ushort maxDamage, DamageType damageType, byte range, ShootType shootType, byte hitChance) : this()
        {
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            DamageType = damageType;
            Range = range;
            ShootType = shootType;
            HitChance = hitChance;
        }

        public byte Range { get; set; }
        public ushort MinDamage { get; set; }
        public ushort MaxDamage { get; set; }
        public DamageType DamageType { get; set; }
        public ShootType ShootType { get; set; }
        public byte HitChance { get; set; }

    }
}
