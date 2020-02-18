//namespace NeoServer.Server.Security
//{
//    public class Xtea
//    {
//        public unsafe static bool Encrypt(NetworkMessage msg, uint[] key)
//        {
//            if (key == null)
//                return false;

//            int pad = msg.Length % 8;
//            if (pad > 0)
//            {
//                msg.AddPaddingBytes(8 - pad);
//            }
//            //msg.AddPaddingBytes(8-pad);
       

//            fixed (byte* bufferPtr = msg.Buffer)
//            {
//                uint* words = (uint*)(bufferPtr + 6); //tirar hardcode 6 e colocar msg.HeaderPosition

//                for (int pos = 0; pos < msg.Length / 4; pos += 2)
//                {
//                    uint x_sum = 0, x_delta = 0x9e3779b9, x_count = 32;

//                    while (x_count-- > 0)
//                    {
//                        words[pos] += (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ x_sum
//                            + key[x_sum & 3];
//                        x_sum += x_delta;
//                        words[pos + 1] += (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ x_sum
//                            + key[x_sum >> 11 & 3];
//                    }
//                }
//            }

//            return true;
//        }

//    }
//}