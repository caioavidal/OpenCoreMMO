using NeoServer.Networking;
using NeoServer.Server.Model.Creatures;
using NeoServer.Server.Model.Items.Contracts;
using NeoServer.Server.Model.Players;
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


        private readonly Func<PlayerModel, Player> _playerFactory;
        private readonly World.Map.Map _map;

        public Game(Func<PlayerModel, Player> playerFactory, World.Map.Map map)
        {
            Connections = new ConcurrentDictionary<uint, Connection>();
            _playerFactory = playerFactory;
            _map = map;
        }

        public Player LogInPlayer(PlayerModel playerRecord, Connection connection)
        {
            var player = _playerFactory(playerRecord);

            _map.AddPlayerOnMap(player);
            _map.AddCreature(player);
         
            return player;
        }

        public Creature GetCreature(uint id)
        {
            return _map.Creatures[id];
        }
      

        public DateTime CombatSynchronizationTime { get; private set; }
    }
}
