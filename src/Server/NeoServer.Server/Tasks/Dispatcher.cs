using NeoServer.Server.Contracts.Tasks;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NeoServer.Server.Tasks
{
    public class Dispatcher : IDispatcher
    {
        private readonly ChannelWriter<IEvent> writer;
        private readonly ChannelReader<IEvent> reader;
        private ulong cycles = 0;

        /// <summary>
        /// A queue responsible for process events
        /// </summary>
        public Dispatcher()
        {
            var channel = Channel.CreateUnbounded<IEvent>(new UnboundedChannelOptions() { SingleReader = true });
            reader = channel.Reader;
            writer = channel.Writer;

        }

        /// <summary>
        /// Adds an event to dispatcher queue
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="hasPriority"></param>
        public void AddEvent(IEvent evt)
        {

            writer.TryWrite(evt);

        }

        public ulong GetCycles()
        {
            return cycles;
        }

        /// <summary>
        /// Starts dispatcher processing queue
        /// </summary>
        /// <param name="token"></param>
        public void Start(CancellationToken token)
        {
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
                            try
                            {
                                evt.Action.Invoke(); //execute event
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace);
                            }
                        }
                    }
                }
            });
        }
    }
}
