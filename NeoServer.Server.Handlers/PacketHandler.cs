using NeoServer.Networking;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Handlers
{
    public abstract class PacketHandler:IPacketHandler
    {
        public abstract void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection);
    }
}
