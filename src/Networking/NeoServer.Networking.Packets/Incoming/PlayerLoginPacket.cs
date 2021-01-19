using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Security;
using System;

namespace NeoServer.Networking.Packets.Incoming
{
    public class PlayerLogInPacket : IncomingPacket
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string CharacterName { get; set; }
        public bool GameMaster { get; set; }
        public byte[] GameServerNonce { get; set; }
        public ushort OS { get; set; }
        public ushort Version { get; set; }
        public PlayerLogInPacket(IReadOnlyNetworkMessage message)
        {
            var packetLength = message.GetUInt16();
            var tcpPayload = packetLength + 2;
            message.SkipBytes(5);

            OS = message.GetUInt16();
            Version = message.GetUInt16();

            //message.SkipBytes(9);

            //// todo: version validation

            var encryptedDataLength = tcpPayload - message.BytesRead;
            var encryptedData = message.GetBytes(encryptedDataLength);
            var data = new ReadOnlyNetworkMessage(RSA.Decrypt(encryptedData), encryptedDataLength);

            LoadXtea(data);

            GameMaster = Convert.ToBoolean(data.GetByte());
            Account = data.GetString();
            CharacterName = data.GetString();
            Password = data.GetString();
            GameServerNonce = data.GetBytes(5);
        }
    }
}
