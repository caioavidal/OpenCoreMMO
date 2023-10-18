using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Creature.Monster.Combat;

public struct IntervalChance : IIntervalChance
{
    public IntervalChance(ushort interval, byte chance)
    {
        Interval = interval;
        Chance = chance;
    }

    public ushort Interval { get; set; }
    public byte Chance { get; set; }
}