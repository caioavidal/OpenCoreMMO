
using NeoServer.Server.Tasks.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Tasks
{
    public class ShedulerEvent : Event, ISchedulerEvent
    {
        public ShedulerEvent(Action action) : base(action)
        {

        }
        public ShedulerEvent(uint delay, Action action) : base(delay, action)
        {
            ExpirationDelay = DateTime.Now.AddMilliseconds(delay).Millisecond;
        }

        public int ExpirationDelay { get; }

        public uint EventId { get; private set; }

        public void SetEventId(uint eventId)
        {
            EventId = eventId;
        }

    
    }
}
