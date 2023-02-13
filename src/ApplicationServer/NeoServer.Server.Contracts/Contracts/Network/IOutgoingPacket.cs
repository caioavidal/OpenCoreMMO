namespace NeoServer.Server.Common.Contracts.Network;

public interface IOutgoingPacket
{
    void WriteToMessage(INetworkMessage message);
}