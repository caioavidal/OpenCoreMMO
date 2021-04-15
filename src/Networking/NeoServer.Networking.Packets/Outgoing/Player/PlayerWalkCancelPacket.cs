using NeoServer.Server.Contracts.Network;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerWalkCancelPacket : OutgoingPacket
    {
        private readonly IPlayer player;

        public PlayerWalkCancelPacket(IPlayer player)
        {
            this.player = player;

        }
        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.PlayerWalkCancel);
            message.AddByte((byte)player.Direction);
        }
    }
}