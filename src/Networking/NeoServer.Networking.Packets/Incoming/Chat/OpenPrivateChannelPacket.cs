using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class OpenPrivateChannelPacket : IncomingPacket
    {
        public OpenPrivateChannelPacket(IReadOnlyNetworkMessage message)
        {
            Receiver = message.GetString();
        }

        public string Receiver { get; set; }
    }
}