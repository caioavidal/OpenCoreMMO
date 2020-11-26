using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class MagicEffectPacket : OutgoingPacket
    {
        private readonly Location location;
        private readonly EffectT effect;
        public MagicEffectPacket(Location location, EffectT effect)
        {
            this.location = location;
            this.effect = effect;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.MagicEffect);
            message.AddLocation(location);
            message.AddByte((byte)effect);
        }
    }
}
