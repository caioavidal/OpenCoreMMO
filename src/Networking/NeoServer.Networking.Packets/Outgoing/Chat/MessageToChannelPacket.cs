using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class MessageToChannelPacket : OutgoingPacket
    {
        public MessageToChannelPacket(ICreature from, SpeechType talkType, string message, ushort channelId)
        {
            From = from;
            TalkType = talkType;
            Message = message;
            ChannelId = channelId;
        }

        public ICreature From { get; }
        public SpeechType TalkType { get; }
        public string Message { get; }
        public ushort ChannelId { get; }
        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.SendPrivateMessage);
            message.AddUInt32(0x00);

            var speechType = TalkType;

            if (speechType == SpeechType.ChannelR2)
            {
                message.AddString(string.Empty);
                speechType = SpeechType.ChannelR1;
            }
            else
            {
                if (From is not null)
                {
                    message.AddString(From.Name);
                }
                else
                {
                    message.AddString(string.Empty);
                }
                //Add level only for players
                if (From is IPlayer player)
                {
                    message.AddUInt16(player.Level);
                }
                else
                {
                    message.AddUInt16(0x00);
                }
            }

            message.AddByte((byte)speechType);
            message.AddUInt16(ChannelId);
            message.AddString(Message);
        }
    }
}
