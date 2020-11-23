using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Contracts.World
{
    public interface ISpawn
    {
        Location Location { get; set; }
        byte Radius { get; set; }

        public interface IMonster
        {
            string Name { get; set; }
            ISpawnPoint Spawn { get; set; }
        }
        IMonster[] Monsters { get; set; }
    }

    public interface ISpawnPoint
    {
        Location Location { get; }
        ushort SpawnTime { get; }
    }
}
