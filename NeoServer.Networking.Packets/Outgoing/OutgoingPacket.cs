using NeoServer.Networking.Packets.Security;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public abstract class OutgoingPacket : IOutgoingPacket
    {
        public virtual bool Disconnect { get; protected set; } = false;
        protected NetworkMessage OutputMessage { get; } = new NetworkMessage();

        private readonly bool AddPayloadLength = true;
        protected OutgoingPacket()
        {
            OutputMessage = new NetworkMessage();
            OutputMessage.AddPayloadLengthSpace();
        }
        protected OutgoingPacket(bool addPayloadLength = true)
        {
            AddPayloadLength = addPayloadLength;

            OutputMessage = new NetworkMessage();

            if (addPayloadLength)
            {
                OutputMessage.AddPayloadLengthSpace();
            }
        }

        public INetworkMessage GetMessage(uint[] xtea)
        {
            if (AddPayloadLength)
            {
                OutputMessage.AddPayloadLength();
            }

            var encrypted = Xtea.Encrypt(OutputMessage, xtea);

            return new GameNetworkMessage(encrypted);
        }
        public INetworkMessage GetMessage()
        {
            if (AddPayloadLength)
            {
                OutputMessage.AddPayloadLength();
            }
            return new GameNetworkMessage(OutputMessage);
        }

    }


}
