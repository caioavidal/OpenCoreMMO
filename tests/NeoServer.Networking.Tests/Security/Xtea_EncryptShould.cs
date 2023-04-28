using System.Linq;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Security;
using Xunit;

namespace NeoServer.Networking.Tests.Security;

public class Xtea_EncryptShould
{
    [Fact]
    public void Encrypt_InputsBytes()
    {
        var input = new NetworkMessage();
        input.AddString("abcdefgh");

        var keys = new uint[4] { 2742731963, 828439173, 895464428, 91929452 };

        var expected = new byte[16] { 85, 203, 81, 215, 209, 61, 121, 160, 65, 229, 232, 33, 111, 86, 232, 158 };

        var encrypted = Xtea.Encrypt(input, keys);

        var encryptedBytes = encrypted.GetMessageInBytes();

        var areEqual = encryptedBytes[..16].ToArray().SequenceEqual(expected);
        Assert.True(areEqual);
    }
}