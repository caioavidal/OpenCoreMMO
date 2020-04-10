using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;

using NeoServer.Server.Tasks.Contracts;

namespace NeoServer.Server.Tasks
{


    public class Scheduler : IScheduler
    {
        private object eventLock = new object();
        private CancellationToken cancellationToken;

        private Queue<ISchedulerEvent> eventQueue = new Queue<ISchedulerEvent>();

        private HashSet<uint> activeEventIds = new HashSet<uint>();

        private uint lastEventId = 0;

        private IDispatcher dispatcher;

        public Scheduler(IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public uint AddEvent(ISchedulerEvent evt)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return 0;
            }

            lock (eventLock)
            {

                if (evt.EventId == 0)
                {
                    evt.SetEventId(GenerateEventId());
                }

                activeEventIds.Add(evt.EventId);
                eventQueue.Enqueue(evt);

                bool shouldPulse = eventQueue.Count == 1;
                if (shouldPulse)
                {
                    Monitor.Pulse(eventLock);
                }
            }

            return evt.EventId;
        }

        public void Start(CancellationToken token)
        {
            cancellationToken = token;

            while (!token.IsCancellationRequested)
            {
                Monitor.Enter(eventLock);


                if (!eventQueue.Any())
                {
                    Monitor.Wait(eventLock);
                }

                //waits the delay expire
                Monitor.Wait(eventLock, eventQueue.Peek().ExpirationDelay);


                if (eventQueue.Any())
                {
                    //ok the event had a timeout and the quere is not empty
                    var evt = eventQueue.Dequeue();

                    if (EventIsCancelled(evt.EventId))
                    {
                        Monitor.Exit(eventLock);
                        continue;
                    }

                    activeEventIds.Remove(evt.EventId);
                    Monitor.Exit(eventLock);

                    evt.SetToNotExpire();

                    dispatcher.AddEvent(evt, true); //send to dispatcher
                }
                else
                {
                    Monitor.Exit(eventLock);
                }
            }
        }

        public bool CancelEvent(uint eventId)
        {
            if (eventId == 0)
            {
                return false;
            }

            lock (eventLock)
            {
                activeEventIds.Remove(eventId);
            }

            return true;
        }

        private bool EventIsCancelled(uint eventId) => !activeEventIds.Contains(eventId);

        private uint GenerateEventId()
        {
            if (++lastEventId == 0)
            {
                return 1;
            }
            return lastEventId;
        }
    }
}