using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Model.Monsters
{

    public struct TargetChange : ITargetChange
    {
        public TargetChange(ushort interval, byte chance)
        {
            Interval = interval;
            Chance = chance;
        }

        public ushort Interval { get; set; }
        public byte Chance { get; set; }
    }
}
