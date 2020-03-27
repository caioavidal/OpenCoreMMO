using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class CreatureMovedPacket : OutgoingPacket
    {
        private readonly Location location;
        private readonly Location toLocation;
        private readonly byte stackPosition;
        public CreatureMovedPacket(Location location, Location toLocation, byte stackPosition)
        {
            this.location = location;
            this.toLocation = toLocation;
            this.stackPosition = stackPosition;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.CreatureMoved);

            message.AddLocation(location);
            message.AddByte(stackPosition);
            message.AddLocation(toLocation);
        }
    }
}
