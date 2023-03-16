using System;
using System.Runtime.InteropServices;
using System.Text;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Network.Enums;

namespace NeoServer.Networking.Packets.Messages;

public class ReadOnlyNetworkMessage : IReadOnlyNetworkMessage
{
    public ReadOnlyNetworkMessage(byte[] buffer, int length)
    {
        if (buffer.IsNull()) return;

        Buffer = buffer;
        Length = length;
        BytesRead = 0;
    }

    public byte[] Buffer { get; protected set; }
    public int Length { get; protected set; }

    /// <summary>
    ///     Get the message's buffer
    /// </summary>
    public byte[] GetMessageInBytes()
    {
        if (Length.IsLessThanZero()) return Array.Empty<byte>();
        return Length == 0 ? Buffer : Buffer[..Length];
    }

    public int BytesRead { get; private set; }

    public GameIncomingPacketType IncomingPacket { get; private set; } = GameIncomingPacketType.None;

    public GameIncomingPacketType GetIncomingPacketType(bool isAuthenticated)
    {
        if (isAuthenticated)
        {
            if (Buffer.Length.IsLessThan(9)) return GameIncomingPacketType.None;

            SkipBytes(6);
            var length = GetUInt16();

            var packetType = (GameIncomingPacketType)GetByte();
            IncomingPacket = packetType;
            return packetType;
        }

        if (Buffer.Length.IsLessThan(6)) return GameIncomingPacketType.None;
        IncomingPacket = (GameIncomingPacketType)Buffer[6];
        return (GameIncomingPacketType)Buffer[6];
    }

    public ushort GetUInt16()
    {
        return Convert(BitConverter.ToUInt16);
    }

    public uint GetUInt32()
    {
        return Convert(BitConverter.ToUInt32);
    }

    public void SkipBytes(int length)
    {
        if (length + BytesRead > Buffer.Length)
            throw new ArgumentOutOfRangeException("Cannot skip bytes that exceeds the buffer length");
        IncreaseByteRead(length);
    }

    public byte GetByte()
    {
        return Convert((_, _) => Buffer[BytesRead]);
    }

    public byte[] GetBytes(int length)
    {
        return Convert((_, _) =>
        {
            var to = BytesRead + length;
            return Buffer[BytesRead..to];
        }, length);
    }

    /// <summary>
    ///     Get string value based on payload length
    /// </summary>
    /// <returns></returns>
    public string GetString()
    {
        var length = GetUInt16();
        if (length == 0 || BytesRead + length > Buffer.Length) return null;

        return Convert((_, _) => Encoding.GetEncoding("iso-8859-1").GetString(Buffer, BytesRead, length), length);
    }

    public void Resize(int length)
    {
        if (length.IsLessThanZero()) return;

        Length = length;
        BytesRead = 0;
    }

    public void Reset()
    {
        Buffer = new byte[16394];
        BytesRead = 0;
        Length = 0;
    }

    public Location GetLocation()
    {
        return new Location { X = GetUInt16(), Y = GetUInt16(), Z = GetByte() };
    }

    private void IncreaseByteRead(int length)
    {
        BytesRead += length;
    }

    private static int SizeOf<T>() where T : struct
    {
        return Marshal.SizeOf(default(T));
    }

    private T Convert<T>(Func<byte[], int, T> converter) where T : struct
    {
        var result = converter(Buffer, BytesRead);
        IncreaseByteRead(SizeOf<T>());
        return result;
    }

    private T Convert<T>(Func<byte[], int, T> converter, int length)
    {
        var result = converter(Buffer, BytesRead);
        IncreaseByteRead(length);
        return result;
    }
}