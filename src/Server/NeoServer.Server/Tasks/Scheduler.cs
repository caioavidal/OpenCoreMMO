using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using NeoServer.Server.Contracts.Tasks;

namespace NeoServer.Server.Tasks
{
    public class Scheduler : IScheduler
    {
        protected readonly ChannelReader<ISchedulerEvent> reader;
        protected readonly ChannelWriter<ISchedulerEvent> writer;

        protected ulong _count;

        protected ConcurrentDictionary<uint, byte> activeEventIds = new();

        protected Channel<ISchedulerEvent> channel;

        private readonly IDispatcher dispatcher;

        protected uint lastEventId;

        public Scheduler(IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            channel = Channel.CreateUnbounded<ISchedulerEvent>(new UnboundedChannelOptions {SingleReader = true});
            reader = channel.Reader;
            writer = channel.Writer;
        }

        public ulong Count => _count;

        public bool Empty => activeEventIds.IsEmpty;

        /// <summary>
        ///     Adds event to be scheduled on the queue
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public virtual uint AddEvent(ISchedulerEvent evt)
        {
            if (evt.EventId == default) evt.SetEventId(++lastEventId);

            if (activeEventIds.TryAdd(evt.EventId, default)) writer.TryWrite(evt);

            return evt.EventId;
        }

        /// <summary>
        ///     Starts scheduler queue
        /// </summary>
        /// <param name="token"></param>
        public virtual void Start(CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (await reader.WaitToReadAsync())
                    // Fast loop around available jobs
                while (reader.TryRead(out var evt))
                {
                    if (EventIsCancelled(evt.EventId)) continue;

                    if (!evt.HasExpired)
                    {
                        ThreadPool.QueueUserWorkItem(o => SendBack(evt));
                        continue;
                    }

                    DispatchEvent(evt);
                }
            });
        }

        /// <summary>
        ///     Cancels event. Event will be not dispatched
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public virtual bool CancelEvent(uint eventId)
        {
            if (eventId == default) return false;
            var removed = activeEventIds.TryRemove(eventId, out _);
            return removed;
        }

        /// <summary>
        ///     Indicates whether event was cancelled
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public bool EventIsCancelled(uint eventId)
        {
            return !activeEventIds.ContainsKey(eventId);
        }

        private void SendBack(ISchedulerEvent evt)
        {
            Thread.Sleep(evt.ExpirationDelay);
            activeEventIds.TryRemove(evt.EventId, out _);
            AddEvent(evt);
        }

        public virtual bool DispatchEvent(ISchedulerEvent evt)
        {
            evt.SetToNotExpire();

            if (!EventIsCancelled(evt.EventId))
            {
                Interlocked.Increment(ref _count);
                activeEventIds.TryRemove(evt.EventId, out _);
                dispatcher.AddEvent(evt); //send to dispatcher      
                return true;
            }

            return false;
        }
    }
}