using NeoServer.Application.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Common.PacketHandler;

public abstract class PacketHandler : IPacketHandler
{
    public abstract void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection);
}