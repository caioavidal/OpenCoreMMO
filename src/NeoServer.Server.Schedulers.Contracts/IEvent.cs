using System;

namespace NeoServer.Server.Tasks.Contracts
{
    public interface IEvent
    {
     
        Action Action { get; }

        bool HasExpired { get; }
        bool HasNoTimeout { get; }

        void SetToNotExpire();
    }
}