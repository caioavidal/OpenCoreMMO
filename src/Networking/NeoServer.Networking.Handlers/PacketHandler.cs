using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Handlers
{
    public abstract class PacketHandler : IPacketHandler
    {
        public abstract void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection);
    }
}
