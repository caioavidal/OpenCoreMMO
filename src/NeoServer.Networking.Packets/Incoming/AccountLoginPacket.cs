using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Security;

namespace NeoServer.Networking.Packets.Incoming
{
    public class AccountLoginPacket : IncomingPacket
    {
        public string Account { get; }
        public string Password { get; }
        public ushort ProtocolVersion { get; }

        public AccountLoginPacket(IReadOnlyNetworkMessage message)
        {
            var packetPayload = message.GetUInt16();
            var tcpPayload = packetPayload + 2;
            message.SkipBytes(7);
            //var os = message.GetUInt16();
            ProtocolVersion = message.GetUInt16();

            message.SkipBytes(12);

            //// todo: version validation

            var encryptedDataLength = tcpPayload - message.BytesRead;
            var encryptedData = message.GetBytes(encryptedDataLength);
            var data = new ReadOnlyNetworkMessage(RSA.Decrypt(encryptedData), encryptedDataLength);

            LoadXtea(data);

            Account = data.GetString();
            Password = data.GetString();
        }

        public bool IsValid() => !(string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(Password));

    }
}
