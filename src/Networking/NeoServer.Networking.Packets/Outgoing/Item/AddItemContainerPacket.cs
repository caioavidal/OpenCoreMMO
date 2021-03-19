using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class AddItemContainerPacket : OutgoingPacket
    {
        private readonly byte containerId;
        private readonly IItem item;
        public AddItemContainerPacket(byte containerId, IItem item)
        {
            this.containerId = containerId;
            this.item = item;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.ContainerAddItem);

            message.AddByte(containerId);
            message.AddItem(item);
        }
    }
}
