using NeoServer.Networking.Packets.Messages;
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
        protected INetworkMessage OutputMessage { get; } = new NetworkMessage();

        private readonly bool AddPayloadLength = true;
     
        protected OutgoingPacket(bool addPayloadLength = true)
        {
            AddPayloadLength = addPayloadLength;

            OutputMessage = new NetworkMessage();

            if (addPayloadLength)
            {
                OutputMessage.AddPayloadLengthSpace();
            }
        }

      
        public INetworkMessage GetMessage()
        {
            if (AddPayloadLength)
            {
                OutputMessage.AddPayloadLength();
            }

            return OutputMessage;
        }
    }


}
