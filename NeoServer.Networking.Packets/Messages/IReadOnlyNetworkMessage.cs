using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Messages
{
    public interface IReadOnlyNetworkMessage: INetworkMessage
    {
        GameIncomingPacketType IncomingPacketType { get; }
        int BytesRead { get; }
        ushort GetUInt16();
        uint GetUInt32();
        void SkipBytes(int count);
        byte GetByte();
        byte[] GetBytes(int length);
        string GetString();
    }
}