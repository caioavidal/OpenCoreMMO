using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using NeoServer.Server.Contracts.Tasks;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NeoServer.Benchmarks.Tasks
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1)]
    public class SchedulerBenchmark
    {
        //[Benchmark]
        //public long WithThreadSleep()
        //{
        //    var scheduler = new SchedulerWithNoDelayThread(null);
        //    var cancellationTokenSource = new CancellationTokenSource();
        //    var cancellationToken = cancellationTokenSource.Token;

        //    scheduler.Start(cancellationToken);

        //    for (int i = 0; i < 10_000_000; i++)
        //    {
        //        scheduler.AddEvent(new SchedulerEvent(10, Action));
        //    }
        //    while (!scheduler.Empty)
        //    {

        //    }
        //    return scheduler.events;
        //}
        [Benchmark]
        public long WithoutDelay()
        {
            var scheduler = new SchedulerWithNoDelay(null);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            scheduler.Start(cancellationToken);

            for (int i = 0; i < 1_000_000; i++)
            {
                scheduler.AddEvent(new SchedulerEvent(10, Action));
            }
            while (!scheduler.Empty)
            {

            }
            return scheduler.events;
        }
        [Benchmark]
        public long WithDelay()
        {
            var scheduler = new SchedulerWithDelay(null);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            scheduler.Start(cancellationToken);

            for (int i = 0; i < 1_000_000; i++)
            {
                scheduler.AddEvent(new SchedulerEvent(10, Action));
            }
            while (!scheduler.Empty)
            {

            }
            return scheduler.events;
        }

        public void Action() { }
    }


    public class SchedulerWithNoDelayThread : Scheduler
    {
        public int events = 0;
        public SchedulerWithNoDelayThread(IDispatcher dispatcher) : base(dispatcher)
        {

        }

        public override void Start(CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (await reader.WaitToReadAsync())
                {
                    // Fast loop around available jobs
                    while (reader.TryRead(out var evt))
                    {
                        if (EventIsCancelled(evt.EventId))
                        {
                            continue;
                        }

                        //await Task.Delay(1);
                        DispatchEvent2(evt);
                    }
                }
            });
        }

        private void DispatchEvent2(ISchedulerEvent evt)
        {
            if (!evt.HasExpired)
            {
                Thread.Sleep(1);
                activeEventIds.TryRemove(evt.EventId, out _);
                AddEvent(evt);
                return;
            }
            evt.SetToNotExpire();

            if (!EventIsCancelled(evt.EventId))
            {
                activeEventIds.TryRemove(evt.EventId, out _);
                events++;
            }
        }
    }

    public class SchedulerWithNoDelay: Scheduler
    {
        public int events = 0;
        public SchedulerWithNoDelay(IDispatcher dispatcher): base(dispatcher)
        {

        }
        
        public override void Start(CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (await reader.WaitToReadAsync())
                {
                    // Fast loop around available jobs
                    while (reader.TryRead(out var evt))
                    {
                        if (EventIsCancelled(evt.EventId))
                        {
                            continue;
                        }

                        //await Task.Delay(1);
                        DispatchEvent2(evt);
                    }
                }
            });
        }

        private async ValueTask DispatchEvent2(ISchedulerEvent evt)
        {
            if (!evt.HasExpired)
            {
                await Task.Delay(1);
                activeEventIds.TryRemove(evt.EventId, out _);
                AddEvent(evt);
                return;
            }
            evt.SetToNotExpire();

            if (!EventIsCancelled(evt.EventId))
            {
                activeEventIds.TryRemove(evt.EventId, out _);
                events++;
            }
        }
    }

    public class SchedulerWithDelay : Scheduler
    {
        public int events = 0;

        public SchedulerWithDelay(IDispatcher dispatcher) : base(dispatcher)
        {

        }

        public override void Start(CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (await reader.WaitToReadAsync())
                {
                    // Fast loop around available jobs
                    while (reader.TryRead(out var evt))
                    {
                        if (EventIsCancelled(evt.EventId))
                        {
                            continue;
                        }

                        DispatchEvent2(evt);
                    }
                }
            });
        }

        private async ValueTask DispatchEvent2(ISchedulerEvent evt)
        {
            await Task.Delay(evt.ExpirationDelay);
            evt.SetToNotExpire();

            if (!EventIsCancelled(evt.EventId))
            {
                activeEventIds.TryRemove(evt.EventId, out _);
                //events++;
            }
        }
    }
}
