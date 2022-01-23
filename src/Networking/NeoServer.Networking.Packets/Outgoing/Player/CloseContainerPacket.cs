using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Player;

public class CloseContainerPacket : OutgoingPacket
{
    private readonly byte containerId;

    public CloseContainerPacket(byte containerId)
    {
        this.containerId = containerId;
    }

    public override void WriteToMessage(INetworkMessage message)
    {
        message.AddByte((byte)GameOutgoingPacketType.ContainerClose);

        message.AddByte(containerId);
    }
}