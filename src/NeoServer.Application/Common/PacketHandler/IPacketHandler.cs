using NeoServer.Networking.Packets.Network;

namespace NeoServer.Application.Common.PacketHandler;

public interface IPacketHandler
{
    void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection);
}