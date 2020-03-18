using NeoServer.Networking.Packets.Security;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class OutgoingPacket : IOutgoingPacket
    {
        protected NetworkMessage OutputMessage { get; } = new NetworkMessage();


        public INetworkMessage GetMessage(uint[] xtea)
        {

            var encrypted = Xtea.Encrypt(OutputMessage, xtea);

            return new GameNetworkMessage(encrypted);
        }
    }

    
}
