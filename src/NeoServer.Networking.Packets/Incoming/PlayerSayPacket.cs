using NeoServer.Game.Common.Talks;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class PlayerSayPacket : IncomingPacket
    {
        public TalkType Talk { get; }
        public string Message { get; }
        public ushort ChannelId { get; set; }

        public PlayerSayPacket(IReadOnlyNetworkMessage message)
        {
            Talk = (TalkType) message.GetByte();
            //ChannelId = message.GetUInt16();
            Message = message.GetString();
        }
    }
}
