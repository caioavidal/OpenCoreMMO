using NeoServer.Game.Common.Talks;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class PlayerSayPacket : IncomingPacket
    {
        public SpeechType Talk { get; }
        public string Receiver{ get; set; }
        public string Message { get; }
        public ushort ChannelId { get; set; }

        public PlayerSayPacket(IReadOnlyNetworkMessage message)
        {
            Talk = (SpeechType) message.GetByte();

            switch (Talk)
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

                case SpeechType.ChannelY:
                case SpeechType.ChannelR1:
                case SpeechType.ChannelO:
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
