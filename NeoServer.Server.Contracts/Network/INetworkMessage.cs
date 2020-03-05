using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Contracts.Network
{
    public interface INetworkMessage
    {
        byte[] GetMessageInBytes(bool addHeader = true);
        int BufferLength { get; }
        int TotalLength { get; }
    }
}
