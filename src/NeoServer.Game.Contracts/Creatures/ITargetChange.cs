namespace NeoServer.Game.Contracts.Creatures
{
    public interface ITargetChange
    {
        byte Chance { get; set; }
        ushort Interval { get; set; }
    }
}
