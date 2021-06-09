using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class CloseChannelPacket : IncomingPacket
    {
        public CloseChannelPacket(IReadOnlyNetworkMessage message)
        {
            ChannelId = message.GetUInt16();
        }

        public ushort ChannelId { get; set; }
    }
}