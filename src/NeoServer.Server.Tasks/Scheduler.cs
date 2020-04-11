using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Autofac;

using NeoServer.Server.Tasks.Contracts;

namespace NeoServer.Server.Tasks
{


    public class Scheduler : IScheduler
    {
        private readonly ChannelWriter<ISchedulerEvent> writer;
        private readonly ChannelReader<ISchedulerEvent> reader;

        private ConcurrentDictionary<uint, byte> cancelledEventIds = new ConcurrentDictionary<uint, byte>();

        private uint lastEventId = 0;

        private IDispatcher dispatcher;

        public Scheduler(IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            var channel = Channel.CreateUnbounded<ISchedulerEvent>(new UnboundedChannelOptions() { SingleReader = true });
            reader = channel.Reader;
            writer = channel.Writer;
        }

        public uint AddEvent(ISchedulerEvent evt)
        {


            if (evt.EventId == default)
            {
                evt.SetEventId(++lastEventId);
            }

            writer.TryWrite(evt);

            return evt.EventId;
        }

        public void Start(CancellationToken token)
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

        private async void DispatchEvent(ISchedulerEvent evt)
        {
            await Task.Delay(evt.ExpirationDelay);
            evt.SetToNotExpire();
            dispatcher.AddEvent(evt, true); //send to dispatcher
        }

        public bool CancelEvent(uint eventId)
        {
            if (eventId == default)
            {
                return false;
            }

            return cancelledEventIds.TryAdd(eventId, default);
        }

        private bool EventIsCancelled(uint eventId) => cancelledEventIds.ContainsKey(eventId);


    }
}