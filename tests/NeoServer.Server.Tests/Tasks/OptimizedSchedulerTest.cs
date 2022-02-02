using System;
using System.Threading;
using Moq;
using NeoServer.Server.Common.Contracts.Tasks;
using NeoServer.Server.Tasks;
using Xunit;

namespace NeoServer.Server.Tests.Tasks;

public class OptimizedSchedulerTest
{
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(100)]
    [InlineData(500)]
    [Theory(Skip = "Only runs manually")]
    public void Start_Must_Execute_Events_After_Delay(int delay)
    {
        var dispatcher = new Mock<IDispatcher>();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var sut = new OptimizedScheduler(dispatcher.Object);

        sut.Start(cancellationToken);

        for (var i = 0; i < 5_000; i++) sut.AddEvent(new SchedulerEvent(delay, () => { }));

        Thread.Sleep(1_000);
        Assert.Equal(5_000ul, sut.Count);
    }

    [Fact(Skip = "Only runs manually")]
    public void Start_Must_Execute_Random_Delayed_Events()
    {
        var dispatcher = new Mock<IDispatcher>();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var sut = new OptimizedScheduler(dispatcher.Object);

        sut.Start(cancellationToken);

        Random random = new();
        for (var i = 0; i < 5_000; i++)
        {
            var delay = random.Next(1, 500);
            sut.AddEvent(new SchedulerEvent(delay, () => { }));
        }

        Thread.Sleep(1_000);
        Assert.Equal(5_000ul, sut.Count);
    }
}