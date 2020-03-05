namespace NeoServer.Networking.Packets.Messages
{
    public interface IReadOnlyNetworkMessage
    {
        GameIncomingPacketType IncomingPacketType { get; }
        int BytesRead { get; }
        ushort GetUInt16();
        uint GetUInt32();
        void SkipBytes(int count);
        void ResetPosition();
        byte GetByte();
        byte[] GetBytes(int length);
        string GetString();
    }
}