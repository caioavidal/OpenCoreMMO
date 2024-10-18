using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler;

public interface IPacketHandler
{
    void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection);
}