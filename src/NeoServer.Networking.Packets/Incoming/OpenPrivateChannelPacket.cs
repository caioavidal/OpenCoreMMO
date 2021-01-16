using NeoServer.Game.Common.Talks;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class OpenPrivateChannelPacket : IncomingPacket
    {
        public string Receiver { get; set; }
        public OpenPrivateChannelPacket(IReadOnlyNetworkMessage message)
        {
            Receiver = message.GetString();
        }
    }
}
