using NeoServer.Game.Common.Talks;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class OpenChannelPacket : IncomingPacket
    {
        public ushort ChannelId { get; set; }
        public OpenChannelPacket(IReadOnlyNetworkMessage message)
        {
            ChannelId = message.GetUInt16();
        }
    }
}
