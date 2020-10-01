using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class TurnToDirectionPacket : OutgoingPacket
    {
        private readonly ICreature creature;
        private readonly Direction direction;
        public TurnToDirectionPacket(ICreature creature, Direction direction)
        {
            this.creature = creature;
            this.direction = direction;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.TransformThing);
            message.AddLocation(creature.Location);
            message.AddByte(creature.GetStackPosition());
            message.AddUInt16(0x63);
            message.AddUInt32(creature.CreatureId);
            message.AddByte((byte)direction);
        }
    }
}
