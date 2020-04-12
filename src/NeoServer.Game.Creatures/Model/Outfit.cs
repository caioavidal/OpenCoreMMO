using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creature.Model
{


    public class Outfit : IOutfit
    {
        public ushort LookType { get; set; }

        public ushort Id { get; set; }

        public byte Head { get; set; }

        public byte Body { get; set; }

        public byte Legs { get; set; }

        public byte Feet { get; set; }
        public byte Addon { get; set; }
    }

}