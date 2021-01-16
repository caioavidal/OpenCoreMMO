using NeoServer.Game.Common.Talks;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class PlayerSayPacket : IncomingPacket
    {
        public TalkType Talk { get; }
        public string Receiver{ get; set; }
        public string Message { get; }
        public ushort ChannelId { get; set; }

        public PlayerSayPacket(IReadOnlyNetworkMessage message)
        {
            Talk = (TalkType) message.GetByte();

            switch (Talk)
            {
                case TalkType.None:
                    return;

                case TalkType.Private:
                case TalkType.PrivateRed:
                #if GAME_FEATURE_RULEVIOLATION
		        case TALKTYPE_RVR_ANSWER:
                #endif
                    Receiver = message.GetString();
                    break;

                case TalkType.ChannelY:
                case TalkType.ChannelR1:
                case TalkType.ChannelO:
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
