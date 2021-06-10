using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Monsters.Combats
{
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
}