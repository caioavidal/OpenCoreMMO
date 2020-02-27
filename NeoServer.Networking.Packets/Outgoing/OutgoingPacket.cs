using NeoServer.Networking.Packets.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class OutgoingPacket
    {
        protected NetworkMessage OutputMessage { get; } = new NetworkMessage();


        public NetworkMessage GetMessage(uint[] xtea)
        {
            Xtea.Encrypt(OutputMessage, xtea);
            return OutputMessage;
        }

    }
}
