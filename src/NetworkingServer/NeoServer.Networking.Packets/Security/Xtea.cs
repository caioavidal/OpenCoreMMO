using System;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Security;

public static class Xtea
{
    public static unsafe INetworkMessage Encrypt(INetworkMessage msg, uint[] key)
    {
        if (key == null)
            throw new ArgumentException("Key invalid");

        var pad = msg.Length % 8;
        if (pad > 0) msg.AddPaddingBytes(8 - pad);

        var buffer = msg.Buffer;
        var length = msg.Length;

        fixed (byte* bufferPtr = buffer)
        {
            var words = (uint*)bufferPtr;

            for (var pos = 0; pos < length / 4; pos += 2)
            {
                const uint xDelta = 0x9e3779b9;
                uint xSum = 0;
                uint xCount = 32;

                while (xCount-- > 0)
                {
                    words[pos] += (((words[pos + 1] << 4) ^ (words[pos + 1] >> 5)) + words[pos + 1]) ^ (xSum
                        + key[xSum & 3]);
                    xSum += xDelta;
                    words[pos + 1] += (((words[pos] << 4) ^ (words[pos] >> 5)) + words[pos]) ^ (xSum
                        + key[(xSum >> 11) & 3]);
                }
            }
        }

        return msg;
    }

    public static unsafe bool Decrypt(IReadOnlyNetworkMessage msg, int index, uint[] key)
    {
        var length = msg.Length;
        var buffer = msg.Buffer;

        if (length <= index || (length - index) % 8 > 0 || key == null) return false;

        fixed (byte* bufferPtr = buffer)
        {
            var words = (uint*)(bufferPtr + index);
            var msgSize = length - index;

            for (var pos = 0; pos < msgSize / 4; pos += 2)
            {
                uint xCount = 32, xSum = 0xC6EF3720, xDelta = 0x9E3779B9;

                while (xCount-- > 0)
                {
                    words[pos + 1] -= (((words[pos] << 4) ^ (words[pos] >> 5)) + words[pos]) ^ (xSum
                        + key[(xSum >> 11) & 3]);
                    xSum -= xDelta;
                    words[pos] -= (((words[pos + 1] << 4) ^ (words[pos + 1] >> 5)) + words[pos + 1]) ^ (xSum
                        + key[xSum & 3]);
                }
            }
        }

        return true;
    }
}