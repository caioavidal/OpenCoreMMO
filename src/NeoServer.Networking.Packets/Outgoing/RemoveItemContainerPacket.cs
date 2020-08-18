using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class RemoveItemContainerPacket : OutgoingPacket
    {
        private readonly byte containerId;
        private readonly byte slotIndex;
        private readonly IItem item;
        public RemoveItemContainerPacket(byte containerId, byte slotIndex, IItem item)
        {
            this.containerId = containerId;
            this.slotIndex = slotIndex;
            this.item = item;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.ContainerRemoveItem);

            message.AddByte(containerId);

            message.AddByte(slotIndex);
        }
    }
}
