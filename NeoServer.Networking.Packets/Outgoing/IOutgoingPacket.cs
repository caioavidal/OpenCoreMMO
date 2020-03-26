using NeoServer.Networking.Packets.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Contracts.Network
{
    public interface IOutgoingPacket
    {
        void WriteToMessage(INetworkMessage message);
    }
}
