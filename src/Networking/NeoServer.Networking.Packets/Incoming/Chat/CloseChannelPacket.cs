using NeoServer.Game.Common.Talks;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class CloseChannelPacket : IncomingPacket
    {
        public ushort ChannelId { get; set; }
        public CloseChannelPacket(IReadOnlyNetworkMessage message)
        {
            ChannelId = message.GetUInt16();
        }
    }
}
