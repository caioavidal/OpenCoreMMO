using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Network.Enums;
using System.Collections.Generic;

namespace NeoServer.Server.Contracts.Network
{
    public interface IReadOnlyNetworkMessage
    {
        byte[] Buffer { get; }

        int Length { get; }
        int BytesRead { get; }
        GameIncomingPacketType IncomingPacket { get; }

        ushort GetUInt16();
        uint GetUInt32();
        void SkipBytes(int count);
        byte GetByte();
        byte[] GetBytes(int length);
        string GetString();
        byte[] GetMessageInBytes();
        GameIncomingPacketType GetIncomingPacketType(bool isAuthenticated);
        void Resize(int length);
        void Reset();
        Location GetLocation();
    }
}