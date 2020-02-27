using System;
using NeoServer.Server.Handlers;
using NeoServer.Server.Model;

namespace NeoServer.Networking.Packets.Incoming
{
    public abstract class IncomingPacket
    {
        public EventHandler<ServerEventArgs> OnIncomingPacket { get; }
        protected byte[] DecryptedMessage { get; }
        public IncomingPacket(IEventHandler handler)
        {
            //var encryptedData = message.GetBytes(tcpPayload - message.BytesRead);
            OnIncomingPacket += handler.OnIncomingMessage;

            //DecryptedMessage = RSA.Decrypt(encryptedData);
        }

        public abstract IServerModel Model { get; }
        public uint[] Xtea { get; } = new uint[4];

        protected void LoadXtea(NetworkMessage message)
        {
            for (int i = 0; i < 4; i++)
            {
                Xtea[i] = message.GetUInt32();
            }
        }
    }
}
