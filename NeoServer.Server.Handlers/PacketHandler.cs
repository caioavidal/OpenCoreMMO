using NeoServer.Networking;
using NeoServer.Networking.Packets.Messages;

namespace NeoServer.Server.Handlers
{
    public abstract class PacketHandler:IPacketHandler
    {
        public abstract void HandlerMessage(IReadOnlyNetworkMessage message, Connection connection);
     
    }
}
