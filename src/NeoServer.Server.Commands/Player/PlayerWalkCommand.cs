using NeoServer.Game.Contracts;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Enums.Location;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using NeoServer.Server.Tasks.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NeoServer.Server.Commands.Player
{

    public class PlayerWalkCommand : Command
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

        public override void Execute()
        {
            player.WalkTo(directions);

            var thing = player as IThing;


            while (!player.WalkingQueue.IsEmpty)
            {
                if (player.WalkingQueue.TryDequeue(out Tuple<byte, Direction> direction))
                {

                    var cooldownMove = Convert.ToInt32(player.StepDelayTicks);
                    game.Scheduler.AddEvent(new ShedulerEvent(150, () =>
                    {
                        MovePlayer(thing, direction);
                    }));
                }

            }
        }

        private void MovePlayer(IThing thing, Tuple<byte, Direction> direction)
        {
            var player = (IPlayer)thing;
            if (player.IsRemoved)
            {
                player.StopWalking();
                return ;
            }
            var toTile = game.Map.GetNextTile(thing.Location, direction.Item2);
            game.Map.MoveThing(ref thing, toTile.Location, 1);
   

        }
    }
}
