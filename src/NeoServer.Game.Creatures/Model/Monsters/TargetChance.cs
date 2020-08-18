using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Model.Monsters
{


    public struct TargetChance : ITargetChance
    {
        public ushort Interval { get; set; }
        public byte Chance { get; set; }
    }
}
