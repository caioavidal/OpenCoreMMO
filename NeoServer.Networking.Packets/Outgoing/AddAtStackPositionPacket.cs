using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class AddAtStackPositionPacket : OutgoingPacket
    {
        private readonly IPlayer player;
        public AddAtStackPositionPacket(IPlayer player)
        {
            this.player = player;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.AddAtStackpos);
            message.AddLocation(player.Location);
            message.AddByte(player.GetStackPosition());
        }
    }
}
