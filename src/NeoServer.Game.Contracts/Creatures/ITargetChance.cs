namespace NeoServer.Game.Contracts.Creatures
{
    public interface ITargetChance
    {
        byte Chance { get; set; }
        ushort Interval { get; set; }
    }
}
