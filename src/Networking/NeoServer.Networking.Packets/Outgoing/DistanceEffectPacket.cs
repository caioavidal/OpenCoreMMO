using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class DistanceEffectPacket : OutgoingPacket
    {
        private readonly Location location;
        private readonly Location toLocation;

        private readonly byte effect;
        public DistanceEffectPacket(Location location, Location toLocation, byte effect)
        {
            this.location = location;
            this.toLocation = toLocation;
            this.effect = effect;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.DistanceShootEffect);
            message.AddLocation(location);
            message.AddLocation(toLocation);
            message.AddByte(effect);
        }
    }
}
