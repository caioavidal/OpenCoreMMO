using NeoServer.Networking;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Handlers
{
    public interface IPacketHandler
    {
        void HandlerMessage(IReadOnlyNetworkMessage message, Connection connection);
    }
}
