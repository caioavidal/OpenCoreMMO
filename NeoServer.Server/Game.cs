using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creature;
using NeoServer.Networking;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;

namespace NeoServer.Server
{
    public class Game
    {
        /// <summary>
        /// Gets the <see cref="ConcurrentDictionary{TKey,TValue}"/> of all <see cref="Connection"/>s in the game, in which the Key is the <see cref="Creature.CreatureId"/>.
        /// </summary>
        private ConcurrentDictionary<uint, Connection> Connections { get; }

        public byte LightLevel => 250;
        public byte LightColor => 215;


        private readonly Func<PlayerModel, IPlayer> _playerFactory;
        public World.Map.Map Map { get; }

        public ICreatureGameInstance CreatureInstances {get;}

        public Game(Func<PlayerModel, IPlayer> playerFactory, World.Map.Map map, 
            ICreatureGameInstance creatureInstances)
        {
            Connections = new ConcurrentDictionary<uint, Connection>();

            _playerFactory = playerFactory;
            Map = map;
            CreatureInstances = creatureInstances;
        }

        public IPlayer LogInPlayer(PlayerModel playerRecord, Connection connection)
        {
            var player = _playerFactory(playerRecord);

            Map.AddPlayerOnMap(player);
            CreatureInstances.Add(player);

            return player;
        }
     
        public DateTime CombatSynchronizationTime { get; private set; }
    }
}
