using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class CloseContainerPacket : OutgoingPacket
    {
        private readonly byte containerId;
        public CloseContainerPacket(byte containerId)
        {

            this.containerId = containerId;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.ContainerClose);

            message.AddByte(containerId);
        }
    }
}
