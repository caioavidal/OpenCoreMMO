using NeoServer.Networking.Connections;
using NeoServer.Server.Handlers;
using NeoServer.Server.Model;
using NeoServer.Server.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Incoming
{
    public abstract class PacketIncoming : IPacketIncoming
    {
        protected byte[] DecryptedMessage { get; }
        public PacketIncoming(NetworkMessage message)
        {
            //var encryptedData = message.GetBytes(tcpPayload - message.BytesRead);


            //DecryptedMessage = RSA.Decrypt(encryptedData);
        }

        public abstract IServerModel Model { get; }
    }
}
