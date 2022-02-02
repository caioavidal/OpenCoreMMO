using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using NeoServer.Server.Common.Contracts.Tasks;
using NeoServer.Server.Tasks;

namespace NeoServer.Benchmarks.Tasks;

[MemoryDiagnoser]
public class SchedulerQueueBenchmark
{
    private readonly AutoResetEvent _autoResetEvent;

    public SchedulerQueueBenchmark()
    {
        _autoResetEvent = new AutoResetEvent(false);
    }

    [Benchmark]
    public void Scheduler()
    {
        DoManyJobs(new SchedulerQueue());
    }

    private void DoManyJobs(ISchedulerQueue<ISchedulerEvent> jobQueue)
    {
        var jobs = 2;
        jobQueue.Start();
        for (var i = 0; i < jobs - 1; i++) jobQueue.AddEvent(new SchedulerEvent(5, () => { }));
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

public class SchedulerQueue : ISchedulerQueue<ISchedulerEvent>
{
    private readonly ConcurrentDictionary<uint, byte> cancelledEventIds = new();
    private readonly ChannelReader<ISchedulerEvent> reader;

    private readonly ChannelWriter<ISchedulerEvent> writer;
    private uint lastId;

    public SchedulerQueue()
    {
        var channel = Channel.CreateUnbounded<ISchedulerEvent>(new UnboundedChannelOptions { SingleReader = true });
        reader = channel.Reader;
        writer = channel.Writer;
    }

    public uint AddEvent(ISchedulerEvent evt)
    {
        if (evt.EventId == default) evt.SetEventId(++lastId);

        writer.TryWrite(evt);

        return evt.EventId;
    }

    public void Start()
    {
        Task.Run(async () =>
        {
            while (await reader.WaitToReadAsync())
                // Fast loop around available jobs
            while (reader.TryRead(out var evt))
            {
                if (EventIsCancelled(evt.EventId)) continue;

                await Task.Delay(evt.ExpirationDelay);

                evt.Action();
            }
        });
    }

    public void Stop()
    {
        writer.Complete();
    }

    public bool CancelEvent(uint eventId)
    {
        if (eventId == default) return false;
        // activeEventIds.Remove(eventId);

        return true;
    }

    private bool EventIsCancelled(uint eventId)
    {
        return cancelledEventIds.ContainsKey(eventId);
    }
}