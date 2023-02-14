namespace NeoServer.Game.Common.Contracts.Creatures;

public interface IIntervalChance
{
    byte Chance { get; set; }
    ushort Interval { get; set; }
}