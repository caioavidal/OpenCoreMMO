using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class WorldLightPacket : OutgoingPacket
    {
        private readonly byte Level;
        private readonly byte Color;
        public WorldLightPacket(byte level, byte color)
        {
            Level = level;
            Color = color;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.WorldLight);

            message.AddByte(Level);
            message.AddByte(Color);
        }
    }
}
