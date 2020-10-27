using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Location;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using System;
using System.Linq;

namespace NeoServer.Server.Commands.Player
{

    public class PlayerWalkCommand //: Command
    {
        private readonly Game game;
        private IPlayer player;
        private readonly Direction[] directions;

        public PlayerWalkCommand(IPlayer player, Game game, params Direction[] directions)
        {
            this.player = player;
            this.game = game;
            this.directions = directions;
        }

        public static void Execute(IPlayer player, Game game, params Direction[] directions)
        {
            player.TryWalkTo(directions);
        }
    }
}
