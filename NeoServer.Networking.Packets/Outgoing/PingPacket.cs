using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Networking.Packets.Messages;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PingPacket : OutgoingPacket
    {
        public PingPacket()
        {
         
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.Ping);
        }
    }
}
