using NeoServer.Server.Model;

namespace NeoServer.Networking.Packets.Incoming
{
    public abstract class IncomingPacket : IPacketIncoming
    {
        protected byte[] DecryptedMessage { get; }
        public IncomingPacket(NetworkMessage message)
        {
            //var encryptedData = message.GetBytes(tcpPayload - message.BytesRead);


            //DecryptedMessage = RSA.Decrypt(encryptedData);
        }

        public abstract IServerModel Model { get; }
    }
}
