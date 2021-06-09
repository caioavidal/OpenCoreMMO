using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerCloseChannelPacket : OutgoingPacket
    {
        private readonly ushort channelId;

        public PlayerCloseChannelPacket(ushort channelId)
        {
            this.channelId = channelId;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte) GameOutgoingPacketType.CloseChannel);
            message.AddUInt16(channelId);
        }
    }
}