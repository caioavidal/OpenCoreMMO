using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class AddAtStackPositionPacket : OutgoingPacket
    {
        private readonly ICreature creature;
        private readonly byte stackPosition;
        public AddAtStackPositionPacket(ICreature creature, byte stackPosition)
        {
            this.creature = creature;
            this.stackPosition = stackPosition;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.AddAtStackpos);
            message.AddLocation(creature.Location);
            message.AddByte(stackPosition);
        }
    }
}
