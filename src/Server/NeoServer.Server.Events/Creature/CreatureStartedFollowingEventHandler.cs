using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts;
using NeoServer.Server.Tasks;
using System.Collections.Generic;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureStartedFollowingEventHandler
    {
        private readonly IGameServer game;
        private IDictionary<uint, uint> followEvents = new Dictionary<uint, uint>();

        public CreatureStartedFollowingEventHandler(IGameServer game)
        {
            this.game = game;
        }
        public void Execute(IWalkableCreature creature, ICreature following, FindPathParams fpp)
        {
            followEvents.TryGetValue(creature.CreatureId, out var followEvent);

            if (followEvent != 0)
            {
                return;
            }

            var eventId = game.Scheduler.AddEvent(new SchedulerEvent(1000, () => Follow(creature, fpp)));
            followEvents.AddOrUpdate(creature.CreatureId, eventId);

        }

        private void Follow(IWalkableCreature creature, FindPathParams fpp)
        {
            followEvents.TryGetValue(creature.CreatureId, out var followEvent);

            if (creature.IsFollowing)
            {
                creature.Follow(creature.Following);
            }
            else
            {
                if (followEvent != 0)
                {
                    game.Scheduler.CancelEvent(followEvent);
                    followEvents.Remove(creature.CreatureId);
                }
            }

            if (followEvent != 0)
            {
                followEvents.Remove(creature.CreatureId);
                Execute(creature, creature.Following, fpp);
            }
        }
    }
}
