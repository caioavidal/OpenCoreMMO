﻿using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Networking.Queue
{
    public class NetworkQueue: INetworkQueue
    {
        private BlockingCollection<OutputStreamMessage> queue;

        public void Enqueue(OutputStreamMessage message)
        {
            queue.Add(message);
        }

        public void Complete() => queue.CompleteAdding();

        public void Start()
        {
            queue = new BlockingCollection<OutputStreamMessage>();
            Task.Run(() =>
            {
                while (!queue.IsCompleted)
                {
                    try
                    {
                        var message = queue.Take();
                        message.Send();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            });
        }
    }
}
