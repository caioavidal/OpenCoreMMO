using Autofac;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Commands;
using NeoServer.Server.Schedulers;
using NeoServer.Server.Schedulers.Contracts;

namespace NeoServer.Game.Commands
{
    public class Dispatcher : IDispatcher
    {

        private readonly IScheduler scheduler;
        public Dispatcher(IScheduler scheduler)
        {
            this.scheduler = scheduler;
        }
        public void Dispatch<TEvent>(TEvent evt) where TEvent : IEvent
        {
            scheduler?.Enqueue(evt);
        }
    }
}