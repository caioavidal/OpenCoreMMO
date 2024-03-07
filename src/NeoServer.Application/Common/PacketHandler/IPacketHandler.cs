using NeoServer.Application.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Common.PacketHandler;

public interface IPacketHandler
{
    void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection);
}