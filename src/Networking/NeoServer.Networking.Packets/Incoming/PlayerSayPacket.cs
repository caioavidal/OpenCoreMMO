using NeoServer.Game.Common.Talks;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class PlayerSayPacket : IncomingPacket
    {
        public SpeechType TalkType { get; }
        public string Receiver{ get; set; }
        public string Message { get; }
        public ushort ChannelId { get; set; }

        public PlayerSayPacket(IReadOnlyNetworkMessage message)
        {
            TalkType = (SpeechType) message.GetByte();

            switch (TalkType)
            {
                case SpeechType.None:
                    return;

                case SpeechType.Private:
                case SpeechType.PrivateRed:
                #if GAME_FEATURE_RULEVIOLATION
		        case TALKTYPE_RVR_ANSWER:
                #endif
                    Receiver = message.GetString();
                    break;

                case SpeechType.ChannelYellowText:
                case SpeechType.ChannelR1:
                case SpeechType.ChannelOrangeText:
                    ChannelId = message.GetUInt16();
                    break;
                default:
                    ChannelId = ushort.MinValue;
                    break;
            }

            Message = message.GetString();
        }
    }
}
