using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Enums.Item;

namespace NeoServer.Game.Contracts.Combat
{
    public interface ICombatAttack
    {
        CombatAttackOption Option { get; }

        ushort CalculateDamage(ushort attackPower, ushort minAttackPower);
        void CauseDamage(ICombatActor actor, ICombatActor enemy);
        void BuildAttack(ICombatActor actor, ICombatActor enemy);
        bool CanAttack(ICombatActor actor, ICombatActor enemy);

        public bool IsMagicalDamage => Option.DamageType switch
        {
            DamageType.Melee => false,
            DamageType.Physical => false,
            _ => true
        };

        public DamageType DamageType => Option.DamageType;
        public bool HasTarget => Option.Target > 0;
        public byte Target => Option.Target;
        //ushort Attack { get; set; }
        //ushort Interval { get; set; }
        //string Name { get; set; }
        //ushort Skill { get; set; }
        //ShootType ShootType { get; }
    }
}
