namespace NeoServer.Server.Common.Contracts.Tasks;

public interface ISchedulerEvent : IEvent
{
    uint EventId { get; }
    int ExpirationDelay { get; }
    double RemainingTime { get; }

    void SetEventId(uint eventId);
}