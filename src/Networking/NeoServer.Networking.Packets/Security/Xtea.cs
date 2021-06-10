using System;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Security
{
    public class Xtea
    {
        public static INetworkMessage Encrypt(INetworkMessage msg, uint[] key)
        {
            if (key == null)
                throw new ArgumentException("Key invalid");

            var pad = msg.Length % 8;
            if (pad > 0) msg.AddPaddingBytes(8 - pad);

            var words = Split(msg.Buffer.AsSpan(0, msg.Length));

            for (var pos = 0; pos < msg.Length / 4; pos += 2)
            {
                uint x_sum = 0, x_delta = 0x9e3779b9, x_count = 32;

                while (x_count-- > 0)
                {
                    words[pos] += (((words[pos + 1] << 4) ^ (words[pos + 1] >> 5)) + words[pos + 1]) ^ (x_sum
                        + key[x_sum & 3]);
                    x_sum += x_delta;
                    words[pos + 1] += (((words[pos] << 4) ^ (words[pos] >> 5)) + words[pos]) ^ (x_sum
                        + key[(x_sum >> 11) & 3]);
                }
            }

            var newBytes = ConvertToBytes(words);

            return new NetworkMessage(newBytes, msg.Length);
        }

        public static unsafe bool Decrypt(IReadOnlyNetworkMessage msg, int index, uint[] key)
        {
            var length = msg.Length;
            var buffer = msg.Buffer;

            if (length <= index || (length - index) % 8 > 0 || key == null) return false;

            fixed (byte* bufferPtr = buffer)
            {
                var words = (uint*) (bufferPtr + index);
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

        private static byte[] ConvertToBytes(Span<uint> array)
        {
            var bytes = new byte[array.Length * 4];
            var index = 0;
            for (var i = 0; i < array.Length; i++)
            {
                var newBytes = BitConverter.GetBytes(array[i]);

                bytes[index] = newBytes[0];
                bytes[index + 1] = newBytes[1];
                bytes[index + 2] = newBytes[2];
                bytes[index + 3] = newBytes[3];

                index += 4;
            }

            return bytes;
        }

        private static Span<uint> Split(ReadOnlySpan<byte> array)
        {
            var newArray = new uint[array.Length / 4];

            var index = 0;
            for (var i = 0; i < array.Length; i += sizeof(uint))
            {
                newArray[index] = BitConverter.ToUInt32(array.Slice(i, 4));
                index++;
            }

            return newArray.AsSpan();
        }
    }
}