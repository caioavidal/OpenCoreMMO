using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming.Chat
{
    public class AddVipPacket : IncomingPacket
    {
        public string Name { get; set; }
        public AddVipPacket(IReadOnlyNetworkMessage message)
        {
            Name = message.GetString();
        }
    }
}
