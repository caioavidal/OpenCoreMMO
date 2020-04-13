using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace NeoServer.Server.Commands.Player
{

    public class PlayerWalkCommand : Command
    {
        private readonly Game game;
        private IPlayer player;
        private readonly Direction[] directions;

        private bool cancelWalking = false;
        public PlayerWalkCommand(IPlayer player, Game game, params Direction[] directions)
        {
            this.player = player;
            this.game = game;
            this.directions = directions;
        }

        public override void Execute()
        {


            if (!player.TryWalkTo(directions))
            {
                game.Scheduler.AddEvent(new SchedulerEvent(player.StepDelayMilliseconds, Execute));
                return;
            }


            //player.OnStoppedWalking += (_) =>
            //{

            //    cancelWalking = true;
            //};

            MovePlayer(player);

        }

        private void MovePlayer(IPlayer player)
        {
           
            var thing = player as IThing;

            if (player.IsRemoved)
            {
                player.StopWalking();
                return;
            }


            if (player.StopWalkingRequested || player.WalkingQueue.IsEmpty)
            {
                Console.WriteLine("cancelled");
                player.NextSteps.ForEach(e => game.Scheduler.CancelEvent(e));
                player.NextSteps.Clear();
               
                //player.NextStepId = default;
                return;
            }

            if (player.WalkingQueue.TryDequeue(out Tuple<byte, Direction> direction))
            {
                var toTile = game.Map.GetNextTile(thing.Location, direction.Item2);
                game.Map.MoveThing(ref thing, toTile.Location, 1);

            }

            var evtId = game.Scheduler.AddEvent(new SchedulerEvent(player.StepDelayMilliseconds, () =>
            {
                MovePlayer(player);
            }));

            player.NextStepId = (byte)evtId;

            player.NextSteps.Add(evtId);
        }
    }
}
