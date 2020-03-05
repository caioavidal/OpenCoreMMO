using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Networking.Packets.Security;
using NeoServer.Server.Model;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class TextMessagePacket : OutgoingPacket
    {
        public TextMessagePacket(string text)
        {
            OutputMessage.AddUInt16((ushort)(text.Length + 3));
            OutputMessage.AddByte(0x0A);
            OutputMessage.AddString(text);
        }
    }
}
