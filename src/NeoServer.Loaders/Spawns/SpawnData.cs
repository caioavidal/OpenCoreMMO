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
            public sbyte Z { get; set; }
            public ushort Spawntime { get; set; }
        }


        public int Centerx { get; set; }
        public int Centery { get; set; }
        public sbyte Centerz { get; set; }
        public byte Radius { get; set; }
        public IEnumerable<Monster> Monsters { get; set; }
    }
}
