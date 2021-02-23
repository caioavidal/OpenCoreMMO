namespace NeoServer.Server.Contracts.Tasks
{
    public interface ISchedulerEvent : IEvent
    {
        uint EventId { get; }

        void SetEventId(uint eventId);
        int ExpirationDelay { get; }
    }
}