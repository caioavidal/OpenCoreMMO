using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
            if (player.NextStepId != 0)
            {
                game.Scheduler.CancelEvent(player.NextStepId);
                player.NextStepId = 0;
            }

            player.StopWalking();


            player.TryWalkTo(directions);
            
            AddEventWalk(directions.Length == 1);
        }

        private void AddEventWalk(bool firstStep)
        {
            var creature = player as ICreature;
            creature.CancelNextWalk = false;

            if (creature.EventWalk != 0)
            {
                return;
            }
            
            if (firstStep)
            {
                MovePlayer(player);
            }

            creature.EventWalk = game.Scheduler.AddEvent(new SchedulerEvent(player.StepDelayMilliseconds, () => MovePlayer(player)));
        }

        private void MovePlayer(ICreature creature)
        {

            var thing = creature as IThing;

            if (creature.TryGetNextStep(out var direction))
            {
                var toTile = game.Map.GetNextTile(thing.Location, direction);
                if (!game.Map.TryMoveThing(ref thing, toTile.Location, 1))
                {
                    player.CancelWalk();
                }
            }
            else
            {
                if (player.EventWalk != 0)
                {
                    game.Scheduler.CancelEvent(player.EventWalk);
                    player.EventWalk = 0;
                }
            }

            if(player.EventWalk != 0)
            {
                player.EventWalk = 0;
                AddEventWalk(false);
            }

            if (creature.IsRemoved)
            {
                creature.StopWalking();
                return;
            }
        }
    }
}
