namespace NeoServer.Server.Contracts.Network
{
    public interface IOutgoingPacket
    {
        void WriteToMessage(INetworkMessage message);
    }
}
