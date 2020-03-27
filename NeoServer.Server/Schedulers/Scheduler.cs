using System;
using System.Collections.Concurrent;
using System.Threading;
using NeoServer.Game.Contracts;

namespace NeoServer.Server.Schedulers
{

    public class Scheduler
    {
        public const int MaxQueueNodes = 3000;
        private ConcurrentQueue<IEvent> events;
        public object _queueLock = new object();
        public Scheduler()
        {
            events = new ConcurrentQueue<IEvent>();
        }

      
        public void Enqueue(IEvent evt)
        {
            events.Enqueue(evt);
            lock (_queueLock)
            {
                Monitor.Pulse(_queueLock);
            }
        }
    

        private void Consume()
        {
            IEvent evt = null;
            if (events.Count == 0 || !events.TryDequeue(out evt))
            {
                lock (_queueLock)
                {
                    Monitor.Wait(_queueLock);
                }
            }

            evt?.Execute();
        }

        public void Start(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Consume();
            }
        }

    }
}