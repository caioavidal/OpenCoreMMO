using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Common.Combat.Structs
{
    public ref struct CombatAttackValue
    {
        public CombatAttackValue(ushort minDamage, ushort maxDamage, DamageType damageType) : this()
        {
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            DamageType = damageType;
        }

        public CombatAttackValue(ushort minDamage, ushort maxDamage, byte range, DamageType damageType)
        {
            Range = range;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            DamageType = damageType;
        }

        public byte Range { get; set; }
        public ushort MinDamage { get; set; }
        public ushort MaxDamage { get; set; }
        public DamageType DamageType { get; set; }
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
        public EffectT EffectT { get; set; }

        public Coordinate[] Area { get; set; }
        public static CombatAttackType None => new CombatAttackType();

    }
}
