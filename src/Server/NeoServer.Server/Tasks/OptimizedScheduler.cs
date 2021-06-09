using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NeoServer.Server.Contracts.Tasks;

namespace NeoServer.Server.Tasks
{
    public class OptimizedScheduler : Scheduler
    {
        private readonly IDispatcher dispatcher;
        protected ConcurrentQueue<ISchedulerEvent> preQueue = new();
        protected object preQueueMonitor = new();

        public OptimizedScheduler(IDispatcher dispatcher) : base(dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public override void Start(CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (await reader.WaitToReadAsync())
                while (reader.TryRead(out var evt))
                {
                    if (EventIsCancelled(evt.EventId)) continue;

                    DispatchEvent(evt);
                }
            });

            Task.Run(() =>
            {
                var replace = new List<ISchedulerEvent>();

                var minDelay = 100;

                while (true)
                {
                    lock (preQueueMonitor)
                    {
                        Monitor.Wait(preQueueMonitor, minDelay);
                    }

                    minDelay = 100;

                    while (preQueue.TryDequeue(out var evt))
                    {
                        var remainingTime = evt.RemainingTime;
                        if (remainingTime > 0)
                        {
                            minDelay = remainingTime < minDelay ? (int) remainingTime : minDelay;
                            replace.Add(evt);
                            continue;
                        }

                        AddEvent(evt);
                    }

                    replace.ForEach(x => preQueue.Enqueue(x));
                    replace.Clear();
                }
            });
        }

        public override bool DispatchEvent(ISchedulerEvent evt)
        {
            if (!evt.HasExpired)
            {
                activeEventIds.TryRemove(evt.EventId, out _);

                preQueue.Enqueue(evt);
                lock (preQueueMonitor)
                {
                    Monitor.Pulse(preQueueMonitor);
                }

                return false;
            }

            evt.SetToNotExpire();

            if (!EventIsCancelled(evt.EventId))
            {
                Interlocked.Increment(ref _count);
                activeEventIds.TryRemove(evt.EventId, out _);
                dispatcher.AddEvent(evt); //send to dispatcher      
                return true;
            }

            return false;
        }
    }
}