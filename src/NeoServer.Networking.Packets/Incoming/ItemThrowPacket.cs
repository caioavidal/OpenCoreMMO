using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class ItemThrowPacket : IncomingPacket
    {
        public Location FromLocation { get; set; }
        public ushort ItemClientId { get; set; }
        public byte FromStackPosition { get; set; }
        public Location ToLocation { get; set; }
        public byte Count { get; set; }
        public ItemThrowPacket(IReadOnlyNetworkMessage message)
        {
            FromLocation = message.GetLocation();
            ItemClientId = message.GetUInt16();
            FromStackPosition = message.GetByte();
            ToLocation = message.GetLocation();
            Count = message.GetByte();
        }
    }
}
