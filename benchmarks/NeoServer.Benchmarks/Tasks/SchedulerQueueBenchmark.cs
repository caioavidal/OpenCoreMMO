using BenchmarkDotNet.Attributes;
using NeoServer.Server.Contracts.Tasks;
using NeoServer.Server.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NeoServer.Benchmarks.Tasks
{
    [MemoryDiagnoser]
    public class SchedulerQueueBenchmark
    {
        private AutoResetEvent _autoResetEvent;

        public SchedulerQueueBenchmark()
        {
            _autoResetEvent = new AutoResetEvent(false);
        }
        [Benchmark]
        public void Scheduler()
        {
            DoManyJobs(new Scheduler());

        }

        private void DoManyJobs(ISchedulerQueue<ISchedulerEvent> jobQueue)
        {
            int jobs = 2;
            jobQueue.Start();
            for (int i = 0; i < jobs - 1; i++)
            {
                jobQueue.AddEvent(new SchedulerEvent(5, () => { }));
            }
            jobQueue.AddEvent(new SchedulerEvent(() => _autoResetEvent.Set()));
            _autoResetEvent.WaitOne();
            jobQueue.Stop();
        }
    }

    internal interface ISchedulerQueue<T>
    {
        uint AddEvent(ISchedulerEvent job);
        void Stop();

        void Start();
    }

    public class Scheduler : ISchedulerQueue<ISchedulerEvent>
    {

        private readonly ChannelWriter<ISchedulerEvent> writer;
        private readonly ChannelReader<ISchedulerEvent> reader;
        private uint lastId = 0;

        private ConcurrentDictionary<uint, byte> cancelledEventIds = new ConcurrentDictionary<uint, byte>();

        public Scheduler()
        {
            var channel = Channel.CreateUnbounded<ISchedulerEvent>(new UnboundedChannelOptions() { SingleReader = true });
            reader = channel.Reader;
            writer = channel.Writer;
        }

        public uint AddEvent(ISchedulerEvent evt)
        {

            if (evt.EventId == default)
            {
                evt.SetEventId(++lastId);

            }

            writer.TryWrite(evt);

            return evt.EventId;
        }

        public void Start()
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

                        await Task.Delay(evt.ExpirationDelay);

                        evt.Action();
                    }
                }
            });
        }
        public void Stop()
        {
            writer.Complete();
        }

        public bool CancelEvent(uint eventId)
        {
            if (eventId == default)
            {
                return false;
            }
            // activeEventIds.Remove(eventId);

            return true;
        }

        private bool EventIsCancelled(uint eventId) => cancelledEventIds.ContainsKey(eventId);

    }

}
