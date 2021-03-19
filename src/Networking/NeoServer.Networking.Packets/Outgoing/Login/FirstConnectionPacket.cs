using NeoServer.Server.Contracts.Network;
using System;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class FirstConnectionPacket : OutgoingPacket
    {
        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddUInt16(0x0006);
            message.AddByte(0x1F);
            message.AddUInt32((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            Random rnd = new Random();
            var bytes = new byte[10];
            rnd.NextBytes(bytes);
            message.AddByte(bytes[0]);
        }
    }
}
