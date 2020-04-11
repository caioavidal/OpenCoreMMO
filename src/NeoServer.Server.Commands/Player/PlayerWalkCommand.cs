using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using System;

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

          
            MovePlayer(thing);
        }

        private void MovePlayer(IThing thing)
        {
            var player = (IPlayer)thing;


            if (player.WalkingQueue.IsEmpty)
            {
                return;
            }

            if (player.IsRemoved)
            {
                player.StopWalking();
                return;
            }

            if (player.WalkingQueue.TryDequeue(out Tuple<byte, Direction> direction))
            {
                var toTile = game.Map.GetNextTile(thing.Location, direction.Item2);
                game.Map.MoveThing(ref thing, toTile.Location, 1);

            }

            Console.WriteLine($"{ player.StepDelayMilliseconds}");

            game.Scheduler.AddEvent(new SchedulerEvent(player.StepDelayMilliseconds, () =>
            {
                MovePlayer(thing);
            }));
        }
    }
}
