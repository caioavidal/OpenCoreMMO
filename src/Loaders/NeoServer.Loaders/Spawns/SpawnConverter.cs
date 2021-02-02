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
            //todo: remove code duplucation
            if (spawnData.Monsters is not null)
            {
                spawn.Monsters = new Spawn.Creature[spawnData.Monsters.Count()];

                var i = 0;
                foreach (var monster in spawnData.Monsters)
                {
                    spawn.Monsters[i++] = new Spawn.Creature
                    {
                        Name = monster.Name,
                        Spawn = new SpawnPoint(new Location((ushort)(monster.X + spawn.Location.X), (ushort)(monster.Y + spawn.Location.Y), monster.Z), monster.Spawntime)
                    };
                }
            }

            if (spawnData.Npcs is not null)
            {
                spawn.Npcs = new Spawn.Creature[spawnData.Npcs.Count()];

                var i = 0;
                foreach (var npc in spawnData.Npcs)
                {
                    spawn.Npcs[i++] = new Spawn.Creature
                    {
                        Name = npc.Name,
                        Spawn = new SpawnPoint(new Location((ushort)(npc.X + spawn.Location.X), (ushort)(npc.Y + spawn.Location.Y), npc.Z), npc.Spawntime)
                    };
                }
            }

            return spawn;
        }
    }
}
