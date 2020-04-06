using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Security;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public abstract class OutgoingPacket : IOutgoingPacket
    {
        public virtual bool Disconnect { get; protected set; } = false;
        public abstract void WriteToMessage(INetworkMessage message);
    }


}
