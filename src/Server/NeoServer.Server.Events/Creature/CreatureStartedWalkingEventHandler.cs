using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureStartedWalkingEventHandler
    {
        private readonly Game game;

        public CreatureStartedWalkingEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(IWalkableCreature creature)
        {
            if (creature.EventWalk != 0)
            {
                return;
            }
            
            creature.EventWalk = game.Scheduler.AddEvent(new SchedulerEvent(creature.StepDelayMilliseconds, () => Move(creature)));
        }
        private void Move(IWalkableCreature creature)
        {
            if (creature.HasNextStep)
            {
                game.Map.MoveCreature(creature);
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
                Execute(creature);
            }
        }
    }
}
