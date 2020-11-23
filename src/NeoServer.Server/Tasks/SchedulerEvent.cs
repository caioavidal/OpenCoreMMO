using NeoServer.Server.Contracts.Tasks;
using System;

namespace NeoServer.Server.Tasks
{
    public class SchedulerEvent : Event, ISchedulerEvent
    {
        public SchedulerEvent(Action action) : base(action)
        {

        }
        public SchedulerEvent(int delay, Action action) : base(delay, action)
        {
            ExpirationDelay = delay;
        }

        /// <summary>
        /// Returns the delay to execute event
        /// </summary>
        public int ExpirationDelay { get; }

        /// <summary>
        /// Event's Id
        /// </summary>
        public uint EventId { get; private set; }

        /// <summary>
        /// Sets the event's Id
        /// </summary>
        /// <param name="eventId"></param>
        public void SetEventId(uint eventId)
        {
            EventId = eventId;
        }
    }
}
