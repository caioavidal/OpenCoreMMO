using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Contracts.Network
{
    public interface IOutgoingPacket
    {
        INetworkMessage GetMessage(uint[] xtea);
    }
}
