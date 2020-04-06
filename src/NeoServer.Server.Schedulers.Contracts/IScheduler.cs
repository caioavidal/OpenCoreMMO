using System.Threading;

namespace NeoServer.Server.Schedulers.Contracts
{
    public interface IScheduler
    {
        void Enqueue<TEvent>(TEvent evt) where TEvent : IEvent;
        void Start(CancellationToken token);
    }
}