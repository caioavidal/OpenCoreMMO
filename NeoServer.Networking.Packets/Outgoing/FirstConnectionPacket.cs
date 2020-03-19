using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Networking.Packets.Security;
using NeoServer.Server.Model;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class FirstConnectionPacket : OutgoingPacket
    {
        public FirstConnectionPacket():base(false)
        {
            AddMessage();
        }

        private void AddMessage()
        {
            OutputMessage.AddUInt16(6);
            OutputMessage.AddByte(0x1F);
            OutputMessage.AddUInt32((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            Random rnd = new Random();
            var bytes = new byte[10];
            rnd.NextBytes(bytes);
            OutputMessage.AddByte(bytes[0]);
        }
     
    }
}
