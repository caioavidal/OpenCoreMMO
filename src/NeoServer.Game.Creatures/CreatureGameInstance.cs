using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NeoServer.Game.Creature
{

    public class CreatureGameInstance : ICreatureGameInstance
    {
        private ConcurrentDictionary<uint, ICreature> _gameInstances;
        private ConcurrentDictionary<uint, Tuple<IMonster, TimeSpan>> _killedMonsters;

        public CreatureGameInstance()
        {
            _gameInstances = new ConcurrentDictionary<uint, ICreature>();
            _killedMonsters = new ConcurrentDictionary<uint, Tuple<IMonster, TimeSpan>>();
        }

        public void AddKilledMonsters(IMonster monster)
        {
            if (!monster.FromSpawn)
            {
                return;
            }

            _killedMonsters.TryAdd(monster.CreatureId, new Tuple<IMonster, TimeSpan>(monster, DateTime.Now.TimeOfDay));
        }

        public bool TryGetCreature(uint id, out ICreature creature) => _gameInstances.TryGetValue(id, out creature);

        public IEnumerable<ICreature> All() => _gameInstances.Values;

        public ImmutableList<Tuple<IMonster, TimeSpan>> AllKilledMonsters() => _killedMonsters.Values.ToImmutableList();

        public void Add(ICreature creature)
        {
            if (!_gameInstances.TryAdd(creature.CreatureId, creature))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to add {creature.Name} to the global dictionary.");
            }
        }

        public bool TryRemoveFromKilledMonsters(uint id)
        {
            if (!_killedMonsters.TryRemove(id, out Tuple<IMonster, TimeSpan> creature))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to remove {creature.Item1.Name} from the killed monsters dictionary.");
                return false;
            }
            return true;
        }

        public bool TryRemove(uint id)
        {
            if (!_gameInstances.TryRemove(id, out ICreature creature))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to remove {creature.Name} from the global dictionary.");
                return false;
            }
            return true;
        }
    }
}