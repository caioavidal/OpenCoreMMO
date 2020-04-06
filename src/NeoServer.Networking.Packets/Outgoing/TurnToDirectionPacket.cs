using NeoServer.Game.Enums.Location;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class TurnToDirectionPacket : OutgoingPacket
    {
        private readonly IPlayer player;
        private readonly Direction direction;
        public TurnToDirectionPacket(IPlayer player, Direction direction)
        {
            this.player = player;
            this.direction = direction;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.TransformThing);
            message.AddLocation(player.Location);
            message.AddByte(player.GetStackPosition());
            message.AddUInt16(0x63);
            message.AddUInt32(player.CreatureId);
            message.AddByte((byte)direction);
        }
    }
}
