using NeoServer.Game.Contracts.Chats;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerChannelListPacket : OutgoingPacket
    {
        private readonly IChatChannel[] chatChannels;

        public PlayerChannelListPacket(IChatChannel[] chatChannels)
        {
            this.chatChannels = chatChannels;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.ChannelList);

            message.AddByte((byte)chatChannels.Length);

            foreach (var channel in chatChannels)
            {
                message.AddUInt16(channel.Id);
                message.AddString(channel.Name);
            }
        }
    }
}
