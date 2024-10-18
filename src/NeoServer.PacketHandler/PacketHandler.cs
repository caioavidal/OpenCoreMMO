using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler;

public abstract class PacketHandler : IPacketHandler
{
    public abstract void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection);
}