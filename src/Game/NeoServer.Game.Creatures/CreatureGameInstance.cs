using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NeoServer.Game.Creature
{

    public class CreatureGameInstance : ICreatureGameInstance
    {
        private readonly Dictionary<uint, ICreature> _creatures;
        private readonly Dictionary<uint, IPlayer> _playersLogged;


        private Dictionary<uint, Tuple<IMonster, TimeSpan>> _killedMonsters;

        public CreatureGameInstance()
        {

            _creatures = new Dictionary<uint, ICreature>();
            _killedMonsters = new Dictionary<uint, Tuple<IMonster, TimeSpan>>();
            _playersLogged = new Dictionary<uint, IPlayer>();
        }

        public void AddKilledMonsters(IMonster monster)
        {
            if (!monster.FromSpawn)
            {
                return;
            }

            _killedMonsters.TryAdd(monster.CreatureId, new Tuple<IMonster, TimeSpan>(monster, DateTime.Now.TimeOfDay));
        }

        public bool TryGetCreature(uint id, out ICreature creature) => _creatures.TryGetValue(id, out creature);
        public bool TryGetPlayer(uint playerId, out IPlayer player) => _playersLogged.TryGetValue(playerId, out player);

        public IEnumerable<ICreature> All() => _creatures.Values;

        public ImmutableList<Tuple<IMonster, TimeSpan>> AllKilledMonsters() => _killedMonsters.Values.ToImmutableList();

        public void Add(ICreature creature)
        {
            if (!_creatures.TryAdd(creature.CreatureId, creature))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to add {creature.Name} to the global dictionary.");
            }
        }
        public void AddPlayer(IPlayer player)
        {
            if (!_playersLogged.TryAdd(player.Id, player))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to add {player.Name} to the global dictionary.");
            }
        }

        public bool TryRemoveFromKilledMonsters(uint id)
        {
            if (!_killedMonsters.Remove(id, out Tuple<IMonster, TimeSpan> creature))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to remove {creature.Item1.Name} from the killed monsters dictionary.");
                return false;
            }
            return true;
        }

        public bool TryRemove(uint id)
        {
            if (!_creatures.Remove(id, out ICreature creature))
            {
                // TODO: proper logging
               // Console.WriteLine($"WARNING: Failed to remove {creature.Name} from the global dictionary.");
                return false;
            }
            return true;
        }
        public bool TryRemoveFromLoggedPlayers(uint id)
        {
            if (!_playersLogged.Remove(id, out IPlayer player))
            {
                // TODO: proper logging
                // Console.WriteLine($"WARNING: Failed to remove {creature.Name} from the global dictionary.");
                return false;
            }
            return true;
        }
    }
}