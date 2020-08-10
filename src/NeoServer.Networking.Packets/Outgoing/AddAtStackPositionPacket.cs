using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class AddAtStackPositionPacket : OutgoingPacket
    {
        private readonly ICreature creature;
        public AddAtStackPositionPacket(ICreature creature)
        {
            this.creature = creature;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.AddAtStackpos);
            message.AddLocation(creature.Location);
            message.AddByte(creature.GetStackPosition());
        }
    }
}
