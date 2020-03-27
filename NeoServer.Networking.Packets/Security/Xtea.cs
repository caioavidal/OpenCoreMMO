using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Networking.Packets.Security
{
    public class Xtea
    {
        public static INetworkMessage Encrypt(INetworkMessage msg, uint[] key)
        {
            if (key == null)
                throw new ArgumentException("Key invalid");

            int pad = msg.Length % 8;
            if (pad > 0)
            {
                msg.AddPaddingBytes(8 - pad);
            }

            var words = Split(msg.GetMessageInBytes()).ToArray();

            for (int pos = 0; pos < msg.Length / 4; pos += 2)
            {
                uint x_sum = 0, x_delta = 0x9e3779b9, x_count = 32;

                while (x_count-- > 0)
                {
                    words[pos] += (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ x_sum
                        + key[x_sum & 3];
                    x_sum += x_delta;
                    words[pos + 1] += (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ x_sum
                        + key[x_sum >> 11 & 3];
                }
            }

            var newBytes = words.SelectMany(x => BitConverter.GetBytes(x)).ToArray();

            return new NetworkMessage(newBytes, msg.Length);

            
        }

        public static unsafe bool Decrypt(IReadOnlyNetworkMessage msg, int index, uint[] key)
        {
            var length = msg.Length;
            var buffer = msg.Buffer;

            if (length <= index || (length - index) % 8 > 0 || key == null)
            {
                return false;
            }

            fixed (byte* bufferPtr = buffer)
            {
                uint* words = (uint*)(bufferPtr + index);
                int msgSize = length - index;

                for (int pos = 0; pos < msgSize / 4; pos += 2)
                {
                    uint xCount = 32, xSum = 0xC6EF3720, xDelta = 0x9E3779B9;

                    while (xCount-- > 0)
                    {
                        words[pos + 1] -= (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ xSum
                            + key[xSum >> 11 & 3];
                        xSum -= xDelta;
                        words[pos] -= (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ xSum
                            + key[xSum & 3];
                    }
                }
            }

            length = BitConverter.ToUInt16(buffer, index) + 2 + index;
            return true;
        }

        private static IEnumerable<uint> Split(byte[] array)
        {
            for (var i = 0; i < array.Length; i += sizeof(uint))
            {
                var to = i + 4;
                yield return BitConverter.ToUInt32(array[i..to], 0);
            }
        }

    }
}