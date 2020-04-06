using System;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Commands;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Schedulers;

namespace NeoServer.Game.Commands
{
    public class PlayerLogInCommandHandler : ICommandHandler<PlayerLogInCommand>
    {
        private readonly Server.Game game;

        private ICreatureGameInstance creatureInstances;

        private readonly Func<PlayerModel, IPlayer> playerFactory;

        private readonly IMap map;

        

        public PlayerLogInCommandHandler(Server.Game game, ICreatureGameInstance creatureInstances, Func<PlayerModel, IPlayer> playerFactory, IMap map)
        {
            this.game = game;
            this.creatureInstances = creatureInstances;
            this.playerFactory = playerFactory;
            this.map = map;
        }

        public async void Execute(PlayerLogInCommand command)
        {
            //var player = game.LogInPlayer(command.Player, command.Connection);

            var player = playerFactory(command.PlayerRecord);

            creatureInstances.Add(player);

            AddPlayerToConnectionPool(command, player);

            map.AddPlayerOnMap(player);
        }


        private void AddPlayerToConnectionPool(PlayerLogInCommand command, IPlayer player)
        {
            command.Connection.PlayerId = player.CreatureId;
            command.Connection.IsAuthenticated = true;
            game.Connections.TryAdd(player.CreatureId, command.Connection);
        }
    }
}