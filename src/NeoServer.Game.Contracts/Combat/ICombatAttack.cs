namespace NeoServer.Game.Contracts.Combat
{
    public interface ICombatAttack
    {
        ushort Attack { get; set; }
        ushort Interval { get; set; }
        string Name { get; set; }
        ushort Skill { get; set; }
    }
}
