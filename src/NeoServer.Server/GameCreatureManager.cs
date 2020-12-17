using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NeoServer.Server
{

    /// <summary>
    /// Control creatures on game
    /// </summary>
    public class GameCreatureManager
    {

        private ICreatureGameInstance creatureInstances;

        private readonly Func<IPlayerModel, IPlayer> playerFactory;

        /// <summary>
        /// Gets all creatures in game
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICreature> GetCreatures() => creatureInstances.All();

        public IEnumerable<ICreature> GetAwakeCreatures() => creatureInstances.All();

        private readonly ConcurrentDictionary<uint, IConnection> playersConnection;
        private IMap map;
        public GameCreatureManager(ICreatureGameInstance creatureInstances, IMap map, Func<IPlayerModel, IPlayer> playerFactory)
        {
            this.creatureInstances = creatureInstances;
            this.map = map;
            this.playerFactory = playerFactory;
            playersConnection = new ConcurrentDictionary<uint, IConnection>();
        }

        /// <summary>
        /// Adds creature to game and to map
        /// </summary>
        /// <param name="creature"></param>
        /// <returns></returns>
        public bool AddCreature(ICreature creature)
        {
            creatureInstances.Add(creature);
            map.AddCreature(creature);

            return true;
        }

        /// <summary>
        /// Adds killed monsters
        /// </summary>
        /// <param name="monster"></param>
        public void AddKilledMonsters(IMonster monster)
        {
            creatureInstances.TryRemove(monster.CreatureId);
            creatureInstances.AddKilledMonsters(monster);
        }

        /// <summary>
        /// Returns all killed monsters
        /// </summary>
        /// <returns></returns>

        public IImmutableList<Tuple<IMonster, TimeSpan>> GetKilledMonsters() => creatureInstances.AllKilledMonsters();

        /// <summary>
        /// Gets creature instance on game
        /// </summary>
        /// <param name="id">Creature Id</param>
        /// <param name="creature">Creature instance</param>
        /// <returns></returns>

        public bool TryGetPlayer(uint id, out IPlayer player)
        {
            player = default;
            if (TryGetCreature(id, out ICreature creature) && creature is IPlayer)
            {
                player = creature as IPlayer;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a creature by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="creature"></param>
        /// <returns></returns>
        public bool TryGetCreature(uint id, out ICreature creature) => creatureInstances.TryGetCreature(id, out creature);

        /// <summary>
        /// Removes creature from game
        /// This method also removes creature from map
        /// </summary>
        /// <param name="creature"></param>
        /// <returns></returns>
        public bool RemoveCreature(ICreature creature)
        {
            if (creature.IsRemoved)
            {
                return false;
            }

            var thing = creature as IThing;

            if (creature is IWalkableCreature walkableCreature)
            {
                map.RemoveThing(thing, walkableCreature.Tile);
            }

            creatureInstances.TryRemove(creature.CreatureId);
            creature.SetAsRemoved();

            //todo remove summons
            return true;
        }

        /// <summary>
        /// Adds player to game
        /// This methods also adds player to map and to connection pool
        /// </summary>
        /// <param name="playerRecord"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public IPlayer AddPlayer(IPlayerModel playerRecord, IConnection connection)
        {
            var player = playerFactory(playerRecord);

            connection.SetConnectionOwner(player);

            playersConnection.TryAdd(player.CreatureId, connection);

            AddCreature(player);

            return player;
        }

        /// <summary>
        /// Removes player from game, map and connection pool
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool RemovePlayer(IPlayer player)
        {
            if (playersConnection.TryRemove(player.CreatureId, out IConnection connection))
            {
                connection.Close();
            }

            RemoveCreature(player);

            return true;
        }

        /// <summary>
        /// Gets the player connection
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public bool GetPlayerConnection(uint playerId, out IConnection connection) => playersConnection.TryGetValue(playerId, out connection);

    }
}
