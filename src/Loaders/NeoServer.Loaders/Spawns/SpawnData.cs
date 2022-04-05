using System;
using System.Collections.Generic;

namespace NeoServer.Loaders.Spawns;

[Serializable]
public class SpawnData
{
    public ushort CenterX { get; set; }
    public ushort CenterY { get; set; }
    public byte CenterZ { get; set; }
    
    public byte Radius { get; set; }
    public IEnumerable<Creature> Monsters { get; set; }
    public IEnumerable<Creature> Npcs { get; set; }

    [Serializable]
    public class Creature
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public byte Z { get; set; }
        public ushort SpawnTime { get; set; }
    }
}