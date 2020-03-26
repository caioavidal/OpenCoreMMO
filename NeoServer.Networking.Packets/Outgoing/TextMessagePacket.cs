using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Security;
using NeoServer.Server.Model;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class TextMessagePacket : OutgoingPacket
    {
        private readonly string text;
        public TextMessagePacket(string text)
        {
            this.text = text;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte(0x0A);
            message.AddString(text);
        }
    }
}
