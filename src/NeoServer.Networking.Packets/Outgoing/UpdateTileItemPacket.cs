using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class UpdateTileItemPacket : OutgoingPacket
    {
        public readonly Location location;
        public readonly byte stackPosition;
        public readonly IItem item;
        public UpdateTileItemPacket(Location location, byte stackPosition, IItem item)
        {
            item.ThrowIfNull();

            this.location = location;
            this.stackPosition = stackPosition;
            this.item = item;
        }
        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.TransformThing);
            message.AddLocation(location);
            message.AddByte(stackPosition);
            message.AddItem(item);
        }
    }
}
