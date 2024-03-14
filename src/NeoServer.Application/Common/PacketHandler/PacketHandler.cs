using NeoServer.Networking.Packets.Network;

namespace NeoServer.Application.Common.PacketHandler;

public abstract class PacketHandler : IPacketHandler
{
    public abstract void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection);
}