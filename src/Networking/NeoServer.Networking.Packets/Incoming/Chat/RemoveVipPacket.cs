using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming.Chat
{
    public class RemoveVipPacket : IncomingPacket
    {
        public uint PlayerId { get; set; }
        public RemoveVipPacket(IReadOnlyNetworkMessage message)
        {
            PlayerId = message.GetUInt32();
        }
    }
}
