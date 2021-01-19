using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using System.IO;

namespace NeoServer.Server.Security
{
    public class RSA
    {
        private static AsymmetricCipherKeyPair asymmetricCipherKeyPair;
        public static byte[] Decrypt(byte[] data)
        {
            var e = new RsaEngine();
            e.Init(false, asymmetricCipherKeyPair.Private);

            return e.ProcessBlock(data, 0, data.Length);
        }

        public static void LoadPem(string basePath)
        {
            AsymmetricCipherKeyPair keyPair;

            using (var reader = File.OpenText(@$"{basePath}/key.pem"))
            {
                keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();

                asymmetricCipherKeyPair = keyPair;
            }
        }
    }
}

