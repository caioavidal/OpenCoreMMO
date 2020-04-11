using System;
using System.Threading;

namespace NeoServer.Server.Tasks.Contracts
{
    public interface IScheduler
    {
        void Start(CancellationToken token);

        uint AddEvent(ISchedulerEvent evt);
        bool CancelEvent(uint eventId);
    }
}