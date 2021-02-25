using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Loggers;
using NeoServer.Server.Contracts.Tasks;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NeoServer.Benchmarks.Tasks
{
    [HardwareCounters(HardwareCounter.TotalCycles)]
    [RankColumn, MemoryDiagnoser]
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1)]
    public class SchedulerBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            // System.Diagnostics.Debugger.Launch();
        }

        [Benchmark]
        public ulong OptimizedScheduler()
        {
            var scheduler = new CustomOptimizedScheduler(null);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            scheduler.Start(cancellationToken);

            for (int i = 0; i < 1000_000; i++)
            {
                scheduler.AddEvent(new SchedulerEvent(100, Action));
                //scheduler.AddEvent(new SchedulerEvent(1000, Action));
                //scheduler.AddEvent(new SchedulerEvent(5000, Action));
            }

            //Thread.Sleep(100);
           // while (scheduler.Count < 1000) { Thread.Sleep(1); }

            return scheduler.Count;
        }
        [Benchmark]
        public ulong SchedulerWithDelayTask()
        {
            var scheduler = new SchedulerWithDelay(null);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            scheduler.Start(cancellationToken);

            for (int i = 0; i < 1000_000; i++)
            {
                scheduler.AddEvent(new SchedulerEvent(100, Action));
                //scheduler.AddEvent(new SchedulerEvent(1000, Action));
                //scheduler.AddEvent(new SchedulerEvent(5000, Action));
            }

           // Thread.Sleep(100);

            //            while (scheduler.Count < 1000) { }
            //Thread.Sleep(1000);
            //Thread.Sleep(100);

            return scheduler.Count;
        }
        [Benchmark]
        public ulong SchedulerWithDelay1Ms()
        {
            var scheduler = new SchedulerWithDelay1Ms(null);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            scheduler.Start(cancellationToken);

            for (int i = 0; i < 1000_000; i++)
            {
                scheduler.AddEvent(new SchedulerEvent(100, Action));
                //scheduler.AddEvent(new SchedulerEvent(1000, Action));
                //scheduler.AddEvent(new SchedulerEvent(5000, Action));
            }
            //while (scheduler.Count < 1000) { Thread.Sleep(1); }
           // Thread.Sleep(100);
            //Thread.Sleep(1000);

            return scheduler.Count;
        }

        public void Action() { }
    }


    public class CustomOptimizedScheduler : OptimizedScheduler
    {
        public CustomOptimizedScheduler(IDispatcher dispatcher) : base(dispatcher)
        {

        }

        public override bool DispatchEvent(ISchedulerEvent evt)
        {
            if (!evt.HasExpired)
            {
                activeEventIds.TryRemove(evt.EventId, out _);

                preQueue.Enqueue(evt);
                lock (preQueueMonitor)                 // Let's now wake up the thread by
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
                return true;
            }

            return false;
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

                Interlocked.Increment(ref _count);
            }
        }
    }


    public class SchedulerWithDelay1Ms : Scheduler
    {
        public int events = 0;

        public SchedulerWithDelay1Ms(IDispatcher dispatcher) : base(dispatcher)
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

                        if (!evt.HasExpired)
                        {
                            SendBack(evt);
                            continue;
                        }
                        DispatchEvent(evt);
                    }
                }
            });
        }
        private async ValueTask SendBack(ISchedulerEvent evt)
        {
            await Task.Delay(1);
            activeEventIds.TryRemove(evt.EventId, out _);
            AddEvent(evt);
        }

        public override bool DispatchEvent(ISchedulerEvent evt)
        {
            evt.SetToNotExpire();

            if (!EventIsCancelled(evt.EventId))
            {
                Interlocked.Increment(ref _count);
                activeEventIds.TryRemove(evt.EventId, out _);
                return true;
            }
            return false;
        }
    }
}
