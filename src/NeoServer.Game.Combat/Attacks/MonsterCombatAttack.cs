using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Combat.Attacks;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Item;

namespace NeoServer.Game.Combat.Attacks
{
   
    public struct MonsterCombatAttack : IMonsterCombatAttack
    {
        // public byte Skill { get; set; }
        //public byte Attack { get; set; }
        public ushort Interval { get; set; }
        public byte Chance { get; set; }
        //public byte Range { get; set; }
        public byte Radius { get; set; }
        public byte Target { get; set; }
        // public ShootType ShootType { get; set; }
        public DamageType DamageType { get; set; }
        public ushort MinDamage { get; set; }
        public ushort MaxDamage { get; set; }
        public byte Spread { get; set; }
        public byte Length { get; set; }
        public EffectT AreaEffect { get; set; }
        public bool IsMelee => DamageType == DamageType.Melee;
        public ICombatAttack CombatAttack { get; set; }

        public CombatAttackValue Translate()
        {
            if (CombatAttack is DistanceCombatAttack distance)
                return new CombatAttackValue(MinDamage, MaxDamage, distance.Range, DamageType);

            return new CombatAttackValue(MinDamage,MaxDamage,DamageType);
        }
    }
}
