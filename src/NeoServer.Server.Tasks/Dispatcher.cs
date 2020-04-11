using NeoServer.Server.Tasks.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NeoServer.Server.Tasks
{
    public class Dispatcher : IDispatcher
    {
        private readonly ChannelWriter<IEvent> writer;
        private readonly ChannelReader<IEvent> reader;
        private CancellationToken cancellationToken;
        private ulong cycles = 0;

        public Dispatcher()
        {
            var channel = Channel.CreateUnbounded<IEvent>(new UnboundedChannelOptions() { SingleReader = true });
            reader = channel.Reader;
            writer = channel.Writer;
        }

        public void AddEvent(IEvent evt, bool hasPriority = false)
        {


            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            writer.TryWrite(evt);
        }


        public ulong GetCycles()
        {
            return cycles;
        }


        public void Start(CancellationToken token)
        {
            cancellationToken = token;

            Task.Run(async () =>
            {
                while (await reader.WaitToReadAsync())
                {
                    if (token.IsCancellationRequested)
                    {
                        writer.Complete();
                    }
                    // Fast loop around available jobs
                    while (reader.TryRead(out var evt))
                    {
                        if (!evt.HasExpired || evt.HasNoTimeout)
                        {
                            ++cycles;
                            evt.Action.Invoke(); //execute event
                        }
                    }
                }
            });
        }
    }
}
