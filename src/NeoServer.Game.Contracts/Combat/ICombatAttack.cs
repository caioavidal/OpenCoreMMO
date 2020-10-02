using NeoServer.Game.Enums.Item;

namespace NeoServer.Game.Contracts.Combat
{
    public interface ICombatAttack
    {
        DamageType DamageType { get; }

        ushort CalculateDamage(ushort attackPower, ushort minAttackPower);

        public bool IsMagicalDamage => DamageType switch
        {
            DamageType.Melee => false,
            DamageType.Physical => false,
            _ => true
        };
        //ushort Attack { get; set; }
        //ushort Interval { get; set; }
        //string Name { get; set; }
        //ushort Skill { get; set; }
        //ShootType ShootType { get; }
    }
}
