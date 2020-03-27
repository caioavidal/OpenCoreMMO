using System;
using System.Collections.Concurrent;
using System.Threading;
using NeoServer.Game.Contracts;

namespace NeoServer.Server.Schedulers
{
    
    public class Scheduler
    {
        public const int MaxQueueNodes = 3000;
        private ConcurrentQueue<Action> actions;
        public object _queueLock = new object();
        public Scheduler()
        {
            actions = new ConcurrentQueue<Action>();
        }
        public void Enqueue(Action action)
        {

            actions.Enqueue(action);
            lock (_queueLock)
            {
                Monitor.Pulse(_queueLock);
            }
        }

        private void Consume()
        {
            Action action = null;
            if (actions.Count == 0 || !actions.TryDequeue(out action))
            {
                lock (_queueLock)
                {
                    Monitor.Wait(_queueLock, TimeSpan.FromSeconds(5));
                }
            }

            action?.Invoke();
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