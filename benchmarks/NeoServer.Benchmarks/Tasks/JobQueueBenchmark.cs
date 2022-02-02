using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace NeoServer.Benchmarks.Tasks;

[MemoryDiagnoser]
public class JobQueueBenchmark
{
    private readonly AutoResetEvent _autoResetEvent;

    public JobQueueBenchmark()
    {
        _autoResetEvent = new AutoResetEvent(false);
    }

    [Benchmark]
    public void BlockingCollectionQueue()
    {
        DoManyJobs(new BlockingCollectionQueue());
    }

    [Benchmark]
    public void ChannelsQueue()
    {
        DoManyJobs(new ChannelsQueue());
    }

    [Benchmark]
    public void ChannelsQueueDedicatedThread()
    {
        DoManyJobs(new ChannelsQueueDedicatedThread());
    }

    private void DoManyJobs(IJobQueue<Action> jobQueue)
    {
        var jobs = 10_000_000;
        for (var i = 0; i < jobs - 1; i++) jobQueue.Enqueue(() => { });
        jobQueue.Enqueue(() => _autoResetEvent.Set());
        _autoResetEvent.WaitOne();
        jobQueue.Stop();
    }
}

internal interface IJobQueue<T>
{
    void Enqueue(Action job);
    void Stop();
}

public class BlockingCollectionQueue : IJobQueue<Action>
{
    private readonly BlockingCollection<Action> _jobs = new(new ConcurrentQueue<Action>());

    public BlockingCollectionQueue()
    {
        var thread = new Thread(OnStart);
        thread.IsBackground = true;
        thread.Start();
    }

    public void Enqueue(Action job)
    {
        _jobs.Add(job);
    }

    public void Stop()
    {
        _jobs.CompleteAdding();
    }

    private void OnStart()
    {
        foreach (var job in _jobs.GetConsumingEnumerable(CancellationToken.None)) job();
    }
}

public class ChannelsQueue : IJobQueue<Action>
{
    private readonly ChannelWriter<Action> _writer;

    public ChannelsQueue()
    {
        var channel = Channel.CreateUnbounded<Action>(new UnboundedChannelOptions { SingleReader = true });
        var reader = channel.Reader;
        _writer = channel.Writer;

        Task.Run(async () =>
        {
            while (await reader.WaitToReadAsync())
                // Fast loop around available jobs
            while (reader.TryRead(out var job))
                job.Invoke();
        });
    }

    public void Enqueue(Action job)
    {
        _writer.TryWrite(job);
    }

    public void Stop()
    {
        _writer.Complete();
    }
}

public class ChannelsQueueDedicatedThread : IJobQueue<Action>
{
    private readonly ChannelWriter<Action> _writer;

    public ChannelsQueueDedicatedThread()
    {
        var channel = Channel.CreateUnbounded<Action>(new UnboundedChannelOptions { SingleReader = true });
        _writer = channel.Writer;
        var reader = channel.Reader;

        var thread = new Thread(() => Start(reader));
        thread.IsBackground = true;
        thread.Start();
    }

    public void Enqueue(Action job)
    {
        _writer.TryWrite(job);
    }

    public void Stop()
    {
        _writer.Complete();
    }

    public async void Start(ChannelReader<Action> reader)
    {
        while (await reader.WaitToReadAsync())
            // Fast loop around available jobs
        while (reader.TryRead(out var job))
            job.Invoke();
    }
}