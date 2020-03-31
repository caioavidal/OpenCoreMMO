using System;
using System.Collections.Concurrent;
using System.Threading;
using Autofac;
using NeoServer.Server.Schedulers.Contracts;

namespace NeoServer.Server.Schedulers
{


    public class Scheduler : IScheduler
    {
        public const int MaxQueueNodes = 3000;
        private ConcurrentQueue<Action> events;
        public object _queueLock = new object();
        private readonly IComponentContext context;

        public Scheduler(IComponentContext context)
        {
            events = new ConcurrentQueue<Action>();
            this.context = context;
        }

        public void Enqueue<TEvent>(TEvent evt) where TEvent : IEvent
        {
            var handler = context.Resolve<IEventHandler<TEvent>>();

            events.Enqueue(() => handler?.Execute(evt));

            lock (_queueLock)
            {
                Monitor.Pulse(_queueLock);
            }
        }


        private void Consume()
        {
            Action evt = null;
            if (events.Count == 0 || !events.TryDequeue(out evt))
            {
                lock (_queueLock)
                {
                    Monitor.Wait(_queueLock);
                }
            }

            evt?.Invoke();
            //Console.WriteLine($"Event invoked");
        }

        public void Start(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Consume();
            }
        }

    }
}