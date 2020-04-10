using NeoServer.Server.Tasks.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NeoServer.Server.Tasks
{
    public class Dispatcher : IDispatcher
    {
        private object eventLock = new object();
        private CancellationToken cancellationToken;

        private List<IEvent> eventList = new List<IEvent>();

        private ulong cycles = 0;

        public void AddEvent(IEvent evt, bool hasPriority = false)
        {
            var pulse = false;
            lock (eventLock)
            {
                pulse = !eventList.Any(); //send a pulse if eventList is empty

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (hasPriority)
                {
                    eventList.Insert(0, evt);
                }
                else
                {
                    eventList.Add(evt);
                }

                if (pulse)
                {
                    Monitor.Pulse(eventLock);
                }
            }
        }

        public ulong GetCycles()
        {
            return cycles;
        }

     
        public void Start(CancellationToken token)
        {
          
            cancellationToken = token;

            while (!token.IsCancellationRequested)
            {
                Monitor.Enter(eventLock); //block threads

                if (!eventList.Any())
                {
                    Monitor.Wait(eventLock);
                }
                if (eventList.Any()) //process eventList
                {
                    
                    var evt = eventList.First();
                    eventList.RemoveAt(0); //todo: too expensive O(n) n= count

                    Monitor.Exit(eventLock); //can release threads after event removed

                    if (!evt.HasExpired || evt.HasNoTimeout)
                    {
                        ++cycles;
                        evt.Action.Invoke(); //execute event
                    }
                }
                else
                {
                    Monitor.Exit(eventLock);
                }
            }
        }
    }
}
