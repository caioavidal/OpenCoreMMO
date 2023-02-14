using System.Threading;

namespace NeoServer.Server.Common.Contracts.Tasks;

public interface IScheduler
{
    void Start(CancellationToken token);

    uint AddEvent(ISchedulerEvent evt);
    bool CancelEvent(uint eventId);
    bool EventIsCancelled(uint eventId);
}