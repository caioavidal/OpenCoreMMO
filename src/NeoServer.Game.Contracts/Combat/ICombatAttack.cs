using NeoServer.Game.Enums.Item;

namespace NeoServer.Game.Contracts.Combat
{
    public interface ICombatAttack
    {
        ushort CalculateDamage(ushort attackPower, ushort minAttackPower);
        //ushort Attack { get; set; }
        //ushort Interval { get; set; }
        //string Name { get; set; }
        //ushort Skill { get; set; }
        //ShootType ShootType { get; }
    }
}
