using System;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Schedulers;

namespace NeoServer.Game.Commands
{
    public class PlayerLogOutCommandHandler : ICommandHandler<PlayerLogOutCommand>
    {
        private readonly Server.Game game;

        private ICreatureGameInstance creatureInstances;

        private readonly IMap map;



        public PlayerLogOutCommandHandler(Server.Game game, ICreatureGameInstance creatureInstances, IMap map)
        {
            this.game = game;
            this.creatureInstances = creatureInstances;
            this.map = map;
        }

        public void Execute(PlayerLogOutCommand command)
        {
            var player = command.Player;
            var thing = player as IThing;

            if (!player.CanLogout)
            {
                //todo logger here
                return;
            }
            
            map.RemoveThing(ref thing, thing.Tile, 1);

            game.LogOutPlayerFromGame(player);
        }
    }
}