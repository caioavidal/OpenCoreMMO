using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureStartedFollowingEventHandler
    {
        private readonly Game game;

        public CreatureStartedFollowingEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(IWalkableCreature creature, ICreature following, FindPathParams fpp)
        {
            if(creature.FollowEvent != 0)
            {
                return;
            }

            var interval = creature.FirstStep ? 0 : 1000;
            creature.FollowEvent = game.Scheduler.AddEvent(new SchedulerEvent(interval, () => Follow(creature, following, fpp)));
        }

        private void Follow(IWalkableCreature creature, ICreature following, FindPathParams fpp)
        {
            if (creature.IsFollowing)
            {
                creature.Follow(following);
            }
            else
            {
                if(creature.FollowEvent != 0)
                {
                    game.Scheduler.CancelEvent(creature.FollowEvent);
                    creature.FollowEvent = 0;
                }
            }

            if (creature.FollowEvent != 0)
            {
                creature.FollowEvent = 0;

                if(creature.Following != following.CreatureId && game.CreatureManager.TryGetCreature(creature.Following, out var newFollowing))
                {
                    Execute(creature, newFollowing as IWalkableCreature, fpp);
                    return;
                }
                Execute(creature, following, fpp);
            }
        }
    }
}
