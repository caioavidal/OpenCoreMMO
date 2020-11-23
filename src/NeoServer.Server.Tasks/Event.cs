using NeoServer.Server.Contracts.Tasks;
using System;

namespace NeoServer.Server.Tasks
{
    public class Event : IEvent
    {
        private TimeSpan expirationTime;
        public Event(Action action)
        {
            Action = action;
            HasNoTimeout = true;
        }
        public Event(int expirationMs, Action action)
        {
            Action = action;
            expirationTime = DateTime.Now.AddMilliseconds(expirationMs).TimeOfDay;
        }

        /// <summary>
        /// Action to be added on event
        /// </summary>
        public Action Action { get; }

        /// <summary>
        /// Set this property when event has no timeout
        /// </summary>
        public bool HasNoTimeout { get; private set; } = false;

        /// <summary>
        /// Indicates whether event has expired
        /// </summary>
        public bool HasExpired => DateTime.Now.TimeOfDay > expirationTime;

        /// <summary>
        /// Sets event to not expire
        /// </summary>
        public void SetToNotExpire()
        {
            HasNoTimeout = true;
        }
    }
}
