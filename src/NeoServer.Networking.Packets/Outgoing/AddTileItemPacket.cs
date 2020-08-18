using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class AddTileItemPacket : OutgoingPacket
    {
        private readonly IItem item;
        private readonly byte stackPosition;
        public AddTileItemPacket(IItem item, byte stackPosition)
        {
            this.item = item;
            this.stackPosition = stackPosition;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.AddAtStackpos);
            message.AddLocation(item.Location);
            message.AddByte(stackPosition);
            message.AddItem(item);
        }
    }
}
