using NeoServer.Server.Tasks.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

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
        public Event(uint expirationMs, Action action)
        {
            Action = action;
            expirationTime = DateTime.Now.AddMilliseconds(expirationMs).TimeOfDay;
        }

        public Action Action { get; }

        public bool HasNoTimeout { get; private set; } = false;
        public bool HasExpired => DateTime.Now.TimeOfDay > expirationTime;
        

        public void SetToNotExpire()
        {
            HasNoTimeout = true;
        }
    }
}
