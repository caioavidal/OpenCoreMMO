using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoServer.Game.World.Spawns
{
    public class SpawnManager
    {

        private readonly World _world;
        private readonly IMap _map;
        private readonly IMonsterFactory _monsterFactory;
        private readonly ICreatureGameInstance _creatureGameInstance;
        public SpawnManager(World world, IMonsterFactory monsterFactory, IMap map, ICreatureGameInstance creatureGameInstance)
        {
            _world = world;

            _monsterFactory = monsterFactory;
            _map = map;
            _creatureGameInstance = creatureGameInstance;
        }



        public void Respawn()
        {
            foreach (var respawn in _creatureGameInstance.AllKilledMonsters())
            {
                var monster = respawn.Item1;
                var deathTime = respawn.Item2;

                var spawnTime = TimeSpan.FromSeconds(monster.Spawn.SpawnTime);

                if (DateTime.Now.TimeOfDay < deathTime + spawnTime)
                {
                    continue;
                }

                if (_map.ArePlayersAround(monster.Location))
                {
                    continue;
                }
                if (_creatureGameInstance.TryRemoveFromKilledMonsters(monster.CreatureId))
                {
                    monster.Reborn();
                }
            }
        }



        public void StartSpawn()
        {
            foreach (var monsterToSpawn in _world.Spawns.SelectMany(x => x.Monsters))
            {
                var monster = _monsterFactory.Create(monsterToSpawn.Name, monsterToSpawn.Spawn);
                monster.SetNewLocation(monsterToSpawn.Spawn.Location);
                _map.AddCreature(monster);

                _creatureGameInstance.Add(monster);
            }
        }
    }
}
