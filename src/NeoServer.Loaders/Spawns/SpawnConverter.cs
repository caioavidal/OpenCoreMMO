using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.World.Spawns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    Location = new Location(monster.X + spawn.Location.X, monster.Y + spawn.Location.Y, monster.Z),
                    Name = monster.Name,
                    SpawnTime = monster.Spawntime
                };
            }

            return spawn;
        }
    }
}
