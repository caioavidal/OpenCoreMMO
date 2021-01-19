using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Server.Contracts.Network;
using System;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class OpenContainerPacket : OutgoingPacket
    {
        private readonly IContainer container;
        private readonly byte containerId;
        public OpenContainerPacket(IContainer container, byte containerId)
        {

            this.container = container;
            this.containerId = containerId;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.ContainerOpen);

            message.AddByte(containerId);
            message.AddItem(container);
            message.AddString(container.Name);
            message.AddByte(container.Capacity);

            message.AddByte(container.HasParent ? 0x01 : 0x00);

            message.AddByte(Math.Min((byte)0xFF, container.SlotsUsed));

            for (byte i = 0; i < container.SlotsUsed; i++)
            {
                message.AddItem(container.Items[i]);
            }
        }
    }
}
