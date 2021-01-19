using System.Collections.Generic;

namespace NeoServer.Loaders.Spawns
{
    public class SpawnData
    {
        public class Monster
        {
            public string Name { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public byte Z { get; set; }
            public ushort Spawntime { get; set; }
        }

        public ushort Centerx { get; set; }
        public ushort Centery { get; set; }
        public byte Centerz { get; set; }
        public byte Radius { get; set; }
        public IEnumerable<Monster> Monsters { get; set; }
    }
}
