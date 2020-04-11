using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;

namespace NeoServer.Server.Commands.Player
{

    public class PlayerWalkCommand : Command
    {
        private readonly Game game;
        private IPlayer player;
        private readonly Direction[] directions;

        private List<uint> events = new List<uint>();
        private bool cancelWalking = false;
        public PlayerWalkCommand(IPlayer player, Game game, params Direction[] directions)
        {
            this.player = player;
            this.game = game;
            this.directions = directions;
        }

        public override void Execute()
        {
            player.WalkTo(directions);

            player.OnStoppedWalking += (_)=>
            {
                events.ForEach(e => game.Scheduler.CancelEvent(e));
                cancelWalking = true;
            };

            MovePlayer(player);


        }

        private void MovePlayer(IPlayer player)
        {
            if (cancelWalking)
            {
                return;
            }
            var thing = player as IThing;



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


            var evtId = game.Scheduler.AddEvent(new SchedulerEvent(player.StepDelayMilliseconds, () =>
            {
                MovePlayer(player);
            }));

            events.Add(evtId);
        }
    }
}
