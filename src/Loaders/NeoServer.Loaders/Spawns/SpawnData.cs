using System.Collections.Generic;

namespace NeoServer.Loaders.Spawns
{
    public class SpawnData
    {
        public ushort Centerx { get; set; }
        public ushort Centery { get; set; }
        public byte Centerz { get; set; }
        public byte Radius { get; set; }
        public IEnumerable<Creature> Monsters { get; set; }
        public IEnumerable<Creature> Npcs { get; set; }

        public class Creature
        {
            public string Name { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public byte Z { get; set; }
            public ushort Spawntime { get; set; }
        }
    }
}