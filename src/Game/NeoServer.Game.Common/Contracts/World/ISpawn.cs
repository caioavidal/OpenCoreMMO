using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Contracts.World
{
    public interface ISpawn
    {
        Location Location { get; set; }
        byte Radius { get; set; }

        public interface ICreature
        {
            string Name { get; set; }
            ISpawnPoint Spawn { get; set; }
        }
        ICreature[] Monsters { get; set; }
        ICreature[] Npcs { get; set; }
    }

    public interface ISpawnPoint
    {
        Location Location { get; }
        ushort SpawnTime { get; }
    }
}
