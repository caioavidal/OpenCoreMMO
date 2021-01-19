using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Handlers
{
    public interface IPacketHandler
    {
        void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection);
    }
}
