using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.World;

namespace NeoServer.Game.World.Spawns
{

    public class Spawn : ISpawn
    {
        public Location Location { get; set; }
        public byte Radius { get; set; }

        public class Creature : ISpawn.ICreature
        {
            public string Name { get; set; }
            public ISpawnPoint Spawn { get; set; }
        }
        public ISpawn.ICreature[] Monsters { get; set; }
        public ISpawn.ICreature[] Npcs { get; set; }
    }

    public class SpawnPoint : ISpawnPoint
    {
        public SpawnPoint(Location location, ushort spawnTime)
        {
            Location = location;
            SpawnTime = spawnTime;
        }

        public Location Location { get; }
        public ushort SpawnTime { get; }
    }
}
