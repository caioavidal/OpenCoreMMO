using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts;
using NeoServer.Server.Tasks;
using System.Collections.Generic;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureStartedWalkingEventHandler
    {
        private readonly IGameServer game;

        private IDictionary<uint, uint> eventWalks = new Dictionary<uint, uint>();

        public CreatureStartedWalkingEventHandler(IGameServer game)
        {
            this.game = game;
        }
        public void Execute(IWalkableCreature creature)
        {
            eventWalks.TryGetValue(creature.CreatureId, out var eventWalk);

            if (eventWalk != 0)
            {
                return;
            }

            var eventId = game.Scheduler.AddEvent(new SchedulerEvent(creature.StepDelay, () => Move(creature)));
            eventWalks.AddOrUpdate(creature.CreatureId, eventId);

        }
        private void Move(IWalkableCreature creature)
        {
            eventWalks.TryGetValue(creature.CreatureId, out var eventWalk);

            if (creature.HasNextStep)
            {
                game.Map.MoveCreature(creature);
            }
            else
            {
                if (eventWalk != 0)
                {
                    game.Scheduler.CancelEvent(eventWalk);
                    eventWalks.Remove(creature.CreatureId);
                }
            }

            if (eventWalk != 0)
            {
                eventWalks.Remove(creature.CreatureId);
                Execute(creature);
            }
        }
    }
}
