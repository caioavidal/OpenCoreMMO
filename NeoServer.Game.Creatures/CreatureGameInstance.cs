using System;
using System.Collections.Concurrent;
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

        public ICreature this[uint id] => GameInstances[id];


        public void Add(ICreature creature)
        {
            if (!GameInstances.TryAdd(creature.CreatureId, creature))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to add {creature.Name} to the global dictionary.");
            }
        }
        public void Remove(uint id)
        {
            if (!GameInstances.TryRemove(id, out ICreature creature))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to add {creature.Name} to the global dictionary.");
            }
        }
    }
}