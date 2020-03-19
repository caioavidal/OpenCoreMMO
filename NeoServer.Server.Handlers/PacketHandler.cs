using NeoServer.Networking;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Handlers
{
    public abstract class PacketHandler:IPacketHandler
    {
        public abstract void HandlerMessage(IReadOnlyNetworkMessage message, Connection connection);
     
    }
}
