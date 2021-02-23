using NeoServer.Server.Contracts.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NeoServer.Server.Tasks
{

    public class Scheduler : IScheduler
    {
        protected readonly ChannelWriter<ISchedulerEvent> writer;
        protected readonly ChannelReader<ISchedulerEvent> reader;

        protected ConcurrentDictionary<uint, byte> activeEventIds = new ConcurrentDictionary<uint, byte>();

        public bool Empty => activeEventIds.IsEmpty;

        private uint lastEventId = 0;

        private IDispatcher dispatcher;

        public Scheduler(IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            var channel = Channel.CreateUnbounded<ISchedulerEvent>(new UnboundedChannelOptions() { SingleReader = true });
            reader = channel.Reader;
            writer = channel.Writer;
        }

        /// <summary>
        /// Adds event to be scheduled on the queue
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public uint AddEvent(ISchedulerEvent evt)
        {

            if (evt.EventId == default)
            {
                evt.SetEventId(++lastEventId);
            }

            if (activeEventIds.TryAdd(evt.EventId, default))
            {
                writer.TryWrite(evt);
            }

            return evt.EventId;
        }

        /// <summary>
        /// Starts scheduler queue
        /// </summary>
        /// <param name="token"></param>

        public virtual void Start(CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (await reader.WaitToReadAsync())
                {
                    // Fast loop around available jobs
                    while (reader.TryRead(out var evt))
                    {
                        if (EventIsCancelled(evt.EventId))
                        {
                            continue;
                        }

                        DispatchEvent(evt);
                    }
                }
            });
        }
      
      

        private async ValueTask DispatchEvent(ISchedulerEvent evt)
        {
            if (!evt.HasExpired)
            {
                await Task.Delay(1);
                activeEventIds.TryRemove(evt.EventId, out _);
                AddEvent(evt);
                return;
            }

            evt.SetToNotExpire();

            if (!EventIsCancelled(evt.EventId))
            {
                activeEventIds.TryRemove(evt.EventId, out _);
                dispatcher.AddEvent(evt); //send to dispatcher      
            }
        }

        /// <summary>
        /// Cancels event. Event will be not dispatched
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public bool CancelEvent(uint eventId)
        {
            if (eventId == default)
            {
                return false;
            }
            var removed = activeEventIds.TryRemove(eventId, out _);
            return removed;
        }

        /// <summary>
        /// Indicates whether event was cancelled
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public bool EventIsCancelled(uint eventId) => !activeEventIds.ContainsKey(eventId);

    }
}