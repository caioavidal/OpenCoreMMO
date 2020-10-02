using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.World.Map.Tiles;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Jobs.Creatures
{
    public class CreatureMovementJob
    {
        private const uint INTERVAL = 1000;
        private static DateTime lastRun;
        public static void Execute(ICreature creature, Game game)
        {

            
            if (creature.IsDead)
            {
                return;
            }
            if (!(creature is IMonster monster))
            {
                return;
            }

            if (creature.NextStepId != 0)
            {
                game.Scheduler.CancelEvent(creature.NextStepId);
                creature.NextStepId = 0;
            }

            //if(game.Map.GetNextTile(player.Location, direction);

            //creature.StopWalking();


            AddEventWalk(creature, game, false);
        }


        private static void AddEventWalk(ICreature creature, Game game, bool firstStep)
        {

            if (creature.EventWalk != 0)
            {
                return;
            }

            if (firstStep)
            {
                MovePlayer(creature, game);
            }

            creature.EventWalk = game.Scheduler.AddEvent(new SchedulerEvent(creature.StepDelayMilliseconds, () => MovePlayer(creature, game)));
        }

        private static void MovePlayer(ICreature creature, Game game)
        {
            var thing = creature as IMoveableThing;

            if (creature.TryGetNextStep(out var direction))
            {
                var toTile = game.Map.GetNextTile(thing.Location, direction);
                if (!game.Map.TryMoveThing(ref thing, toTile.Location))
                {
                   // creature.CancelWalk();
                }
            }
            else
            {
                if (creature.EventWalk != 0)
                {
                    game.Scheduler.CancelEvent(creature.EventWalk);
                    creature.EventWalk = 0;
                }
            }

            if (creature.EventWalk != 0)
            {
                creature.EventWalk = 0;
                AddEventWalk(creature, game, false);
            }

            if (creature.IsRemoved)
            {
                creature.StopWalking();
                return;
            }
        }
    }
}
