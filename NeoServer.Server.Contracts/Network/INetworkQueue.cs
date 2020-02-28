using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Contracts.Network
{
    public interface INetworkQueue
    {
        void Enqueue(object message);
    }
}
