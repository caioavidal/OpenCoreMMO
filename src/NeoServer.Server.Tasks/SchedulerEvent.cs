
using NeoServer.Server.Tasks.Contracts;
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

        public int ExpirationDelay { get; }

        public uint EventId { get; private set; }

        public void SetEventId(uint eventId)
        {
            EventId = eventId;
        }


    }
}
