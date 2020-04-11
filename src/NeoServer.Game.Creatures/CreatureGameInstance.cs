using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creature
{


    public class CreatureGameInstance : ICreatureGameInstance
    {
        private ConcurrentDictionary<uint, ICreature> GameInstances { get; set; }

        public CreatureGameInstance()
        {
            GameInstances = new ConcurrentDictionary<uint, ICreature>();
        }

        public bool TryGetCreature(uint id, out ICreature creature) => GameInstances.TryGetValue(id, out creature);




        public IEnumerable<ICreature> All() => GameInstances.Values;

        public void Add(ICreature creature)
        {
            if (!GameInstances.TryAdd(creature.CreatureId, creature))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to add {creature.Name} to the global dictionary.");
            }
        }
        public bool TryRemove(uint id)
        {
            if (!GameInstances.TryRemove(id, out ICreature creature))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to add {creature.Name} to the global dictionary.");
                return false;
            }
            return true;
        }
    }
}