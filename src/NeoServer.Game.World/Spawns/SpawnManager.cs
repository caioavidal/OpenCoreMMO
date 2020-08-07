using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoServer.Game.World.Spawns
{
    public class SpawnManager
    {

        private IList<IMonster> _monstersToRespawn = new List<IMonster>();
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

        public void StartSpawn()
        {
            foreach (var monsterToSpawn in _world.Spawns.SelectMany(x => x.Monsters))
            {


                var monster = _monsterFactory.Create(monsterToSpawn.Name);
                monster.SetNewLocation(monsterToSpawn.Location);
                _map.AddCreature(monster);

                _creatureGameInstance.Add(monster);
            }
        }
    }
}
