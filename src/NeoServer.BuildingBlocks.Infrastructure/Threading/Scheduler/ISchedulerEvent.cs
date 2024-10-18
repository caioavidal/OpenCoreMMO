using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;

namespace NeoServer.BuildingBlocks.Infrastructure.Threading.Scheduler;

public interface ISchedulerEvent : IEvent
{
    uint EventId { get; }
    int ExpirationDelay { get; }
    double RemainingTime { get; }

    void SetEventId(uint eventId);
}