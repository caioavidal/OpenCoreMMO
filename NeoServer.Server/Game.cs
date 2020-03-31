using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NeoServer.Server
{
    public class Game
    {
        /// <summary>
        /// Gets the <see cref="ConcurrentDictionary{TKey,TValue}"/> of all <see cref="Connection"/>s in the game, in which the Key is the <see cref="Creature.CreatureId"/>.
        /// </summary>
        public ConcurrentDictionary<uint, IConnection> Connections { get; }

        public byte LightLevel => 250;
        public byte LightColor => 215;


        private readonly Func<PlayerModel, IPlayer> _playerFactory;
        public IMap Map { get; }

        public ICreatureGameInstance CreatureInstances { get; }

        public Game(Func<PlayerModel, IPlayer> playerFactory, IMap map,
            ICreatureGameInstance creatureInstances)
        {
            Connections = new ConcurrentDictionary<uint, IConnection>();

            _playerFactory = playerFactory;
            Map = map;
            CreatureInstances = creatureInstances;
        }

        public IPlayer LogInPlayer(PlayerModel playerRecord, IConnection connection)
        {
            var player = _playerFactory(playerRecord);

            Map.AddPlayerOnMap(player);
            CreatureInstances.Add(player);

            Connections.TryAdd(player.CreatureId, connection);

            return player;
        }

        /// <summary>
        /// Removes player from game instances, connection pool and close player connection;
        /// </summary>

        public bool LogOutPlayerFromGame(IPlayer player)
        {
            if (!CreatureInstances.TryRemove(player.CreatureId))
            {
                return false;
            }

            IConnection connection = null;
            if (!Connections.TryRemove(player.CreatureId, out connection))
            {
                return false;
            }

            connection.Close();
            
            return true;
        }

        public DateTime CombatSynchronizationTime { get; private set; }
    }
}
