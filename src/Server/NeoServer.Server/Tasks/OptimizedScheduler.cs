using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NeoServer.Server.Common.Contracts.Tasks;

namespace NeoServer.Server.Tasks;

public class OptimizedScheduler : Scheduler
{
    private readonly IDispatcher dispatcher;
    protected readonly ConcurrentQueue<ISchedulerEvent> PreQueue = new();
    protected readonly object PreQueueMonitor = new();

    public OptimizedScheduler(IDispatcher dispatcher) : base(dispatcher)
    {
        this.dispatcher = dispatcher;
    }

    public override void Start(CancellationToken token)
    {
        Task.Run(async () =>
        {
            while (await Reader.WaitToReadAsync(token))
            while (Reader.TryRead(out var evt))
            {
                if (EventIsCancelled(evt.EventId)) continue;

                DispatchEvent(evt);
            }
        }, token);

        Task.Run(() =>
        {
            var replace = new List<ISchedulerEvent>();

            const int minDelay = 100;

            PreQueueLoop(minDelay, replace, token);
        }, token);
    }

    private void PreQueueLoop(int minDelay, List<ISchedulerEvent> replace, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            lock (PreQueueMonitor)
            {
                Monitor.Wait(PreQueueMonitor, minDelay);
            }

            minDelay = 100;

            while (PreQueue.TryDequeue(out var evt))
            {
                var remainingTime = evt.RemainingTime;
                if (remainingTime > 0)
                {
                    minDelay = remainingTime < minDelay ? (int)remainingTime : minDelay;
                    replace.Add(evt);
                    continue;
                }

                AddEvent(evt);
            }

            replace.ForEach(x => PreQueue.Enqueue(x));
            replace.Clear();
        }
    }

    protected override bool DispatchEvent(ISchedulerEvent evt)
    {
        if (!evt.HasExpired)
        {
            ActiveEventIds.TryRemove(evt.EventId, out _);

            PreQueue.Enqueue(evt);
            lock (PreQueueMonitor)
            {
                Monitor.Pulse(PreQueueMonitor);
            }

            return false;
        }

        evt.SetToNotExpire();

        if (EventIsCancelled(evt.EventId)) return false;
        
        Interlocked.Increment(ref EventLength);
        ActiveEventIds.TryRemove(evt.EventId, out _);
        dispatcher.AddEvent(evt); //send to dispatcher      
        
        return true;
    }
}