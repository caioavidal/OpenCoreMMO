using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class OpenChannelPacket : IncomingPacket
    {
        public OpenChannelPacket(IReadOnlyNetworkMessage message)
        {
            ChannelId = message.GetUInt16();
        }

        public ushort ChannelId { get; set; }
    }
}