using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Handlers;

public interface IPacketHandler
{
    void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection);
}