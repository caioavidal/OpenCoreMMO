using NeoServer.Server.Tasks.Contracts;
using System;

namespace NeoServer.Server.Tasks.Contracts
{
    public interface ISchedulerEvent : IEvent
    {
        uint EventId { get; }
   
        void SetEventId(uint eventId);
        int ExpirationDelay { get; }
    }
}