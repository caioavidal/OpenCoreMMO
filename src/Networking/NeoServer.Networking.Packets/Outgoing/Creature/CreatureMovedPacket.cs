using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class CreatureMovedPacket : OutgoingPacket
    {
        private readonly Location location;
        private readonly byte stackPosition;
        private readonly Location toLocation;

        public CreatureMovedPacket(Location location, Location toLocation, byte stackPosition)
        {
            this.location = location;
            this.toLocation = toLocation;
            this.stackPosition = stackPosition;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte) GameOutgoingPacketType.CreatureMoved);

            message.AddLocation(location);
            message.AddByte(stackPosition);
            message.AddLocation(toLocation);
        }
    }
}