namespace NeoServer.Networking.Packets.Network;

public interface IOutgoingPacket
{
    void WriteToMessage(INetworkMessage message);
}