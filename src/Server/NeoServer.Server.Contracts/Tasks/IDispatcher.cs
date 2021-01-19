using System.Threading;

namespace NeoServer.Server.Contracts.Tasks
{
    public interface IDispatcher
    {

        void AddEvent(IEvent evt);
        ulong GetCycles();

        void Start(CancellationToken token);
    }
}
