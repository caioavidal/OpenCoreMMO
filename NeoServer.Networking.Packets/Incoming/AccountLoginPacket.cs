using NeoServer.Networking.Connections;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Incoming
{
    public class AccountLoginPacket : PacketIncoming
    {
        public AccountLoginPacket(NetworkMessage message): base(message)
        {
            var packetPayload = message.GetUInt16();
            var tcpPayload = packetPayload + 2;
            message.SkipBytes(7);
            //var os = message.GetUInt16();
            Version = message.GetUInt16();

            //var files = message.GetBytes(12);

            // todo: version validation

            message.SkipBytes(10);

            var encryptedData = message.GetBytes(tcpPayload - message.BytesRead);


            //var decryptedData = new InputMessage(RSA.Decrypt(encryptedData));


            //LoadXtea(decryptedData);
            //LoadAccount(decryptedData);
        }

        public string AccountName { get; }
        public string Password { get; }

        public int Version { get; }
    }
}
