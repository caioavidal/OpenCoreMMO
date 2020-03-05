using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Networking.Packets.Security
{
    public class Xtea
    {

        //     public unsafe static bool Encrypt(NetworkMessage msg, uint[] key)
        //    {
        //        if (key == null)
        //            return false;

        //        int pad = msg.BufferLength % 8;
        //        if (pad > 0)
        //        {
        //            msg.AddPaddingBytes(8 - pad);
        //        }

        //        fixed (byte* bufferPtr = msg.Buffer)
        //        {
        //            uint* words = (uint*)(bufferPtr + 0); //tirar hardcode 0 e colocar msg.HeaderPosition

        //            for (int pos = 0; pos < msg.BufferLength / 4; pos += 2)
        //            {
        //                uint x_sum = 0, x_delta = 0x9e3779b9, x_count = 32;

        //                while (x_count-- > 0)
        //                {
        //                    words[pos] += (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ x_sum
        //                        + key[x_sum & 3];
        //                    x_sum += x_delta;
        //                    words[pos + 1] += (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ x_sum
        //                        + key[x_sum >> 11 & 3];
        //                }
        //            }
        //        }

        //        return true;
        //    }
        public static NetworkMessage Encrypt(NetworkMessage msg, uint[] key)
        {
            if (key == null)
                throw new ArgumentException("Key invalid");

            int pad = msg.BufferLength % 8;
            if (pad > 0)
            {
                msg.AddPaddingBytes(8 - pad);
            }

            var words1 = Split(msg.Buffer[0..msg.BufferLength]).ToArray();

            for (int pos = 0; pos < msg.BufferLength / 4; pos += 2)
            {
                uint x_sum = 0, x_delta = 0x9e3779b9, x_count = 32;

                while (x_count-- > 0)
                {
                    words1[pos] += (words1[pos + 1] << 4 ^ words1[pos + 1] >> 5) + words1[pos + 1] ^ x_sum
                        + key[x_sum & 3];
                    x_sum += x_delta;
                    words1[pos + 1] += (words1[pos] << 4 ^ words1[pos] >> 5) + words1[pos] ^ x_sum
                        + key[x_sum >> 11 & 3];
                }
            }

            var newBytes = words1.SelectMany(x => BitConverter.GetBytes(x)).ToArray();

            var encrypted = new NetworkMessage();

            foreach (var item in newBytes)
            {
                encrypted.AddByte(item);
            }
            return encrypted;

            // fixed (byte* bufferPtr = msg.Buffer)
            // {
            //     uint* words = (uint*)(bufferPtr + 0); //tirar hardcode 0 e colocar msg.HeaderPosition

            //     for (int pos = 0; pos < msg.BufferLength / 4; pos += 2)
            //     {
            //         uint x_sum = 0, x_delta = 0x9e3779b9, x_count = 32;

            //         while (x_count-- > 0)
            //         {
            //             words[pos] += (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ x_sum
            //                 + key[x_sum & 3];
            //             x_sum += x_delta;
            //             words[pos + 1] += (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ x_sum
            //                 + key[x_sum >> 11 & 3];
            //         }
            //     }
            // }

            
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