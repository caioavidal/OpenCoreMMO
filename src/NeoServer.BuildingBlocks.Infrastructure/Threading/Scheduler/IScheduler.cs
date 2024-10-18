namespace NeoServer.BuildingBlocks.Infrastructure.Threading.Scheduler;

public interface IScheduler
{
    void Start(CancellationToken token);

    uint AddEvent(ISchedulerEvent evt);
    bool CancelEvent(uint eventId);
    bool EventIsCancelled(uint eventId);
}