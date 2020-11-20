using System.Threading;

namespace NeoServer.Server.Tasks.Contracts
{
    public interface IDispatcher
    {

        void AddEvent(IEvent evt);
        ulong GetCycles();

        void Start(CancellationToken token);
    }
}
