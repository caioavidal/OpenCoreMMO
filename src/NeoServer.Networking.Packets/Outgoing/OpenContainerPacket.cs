using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class OpenContainerPacket : OutgoingPacket
    {
        private readonly IContainerItem container;
        public OpenContainerPacket(IContainerItem container)
        {
            
            this.container = container;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.ContainerOpen);

            message.AddByte(container.Id);
            message.AddItem(container);
            message.AddString(container.Name);
            message.AddByte(container.Capacity);

            message.AddByte(container.HasParent ? (byte)0x01 : (byte)0x00);

            message.AddByte(Math.Min((byte)0xFF, container.SlotsUsed));

            for (byte i = 0; i < container.SlotsUsed; i++)
            {
                message.AddItem(container.Items[i]);
            }
        }
    }
}
