using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.World.Spawns;
using System.Linq;

namespace NeoServer.Loaders.Spawns
{
    public class SpawnConverter
    {
        public static Spawn Convert(SpawnData spawnData)
        {
            var spawn = new Spawn
            {
                Location = new Location(spawnData.Centerx, spawnData.Centery, spawnData.Centerz),
                Radius = spawnData.Radius,
            };

            if (spawnData.Monsters == null)
            {
                return spawn;
            }

            spawn.Monsters = new Spawn.Monster[spawnData.Monsters.Count()];

            var i = 0;
            foreach (var monster in spawnData.Monsters)
            {
                spawn.Monsters[i++] = new Spawn.Monster
                {
                    Name = monster.Name,
                    Spawn = new SpawnPoint(new Location((ushort)(monster.X + spawn.Location.X), (ushort)(monster.Y + spawn.Location.Y), monster.Z), monster.Spawntime)
                };
            }

            return spawn;
        }
    }
}
