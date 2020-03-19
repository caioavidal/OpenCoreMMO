using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Contracts.Network
{
    public interface IOutgoingPacket
    {
        bool Disconnect { get; }
        INetworkMessage GetMessage(uint[] xtea);
        INetworkMessage GetMessage();
    }
}
