using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class MagicEffectPacket: OutgoingPacket
    {
        public MagicEffectPacket(Location location, EffectT effect) : base(false)
        {
            OutputMessage.AddByte((byte)GameOutgoingPacketType.MagicEffect);
            OutputMessage.AddLocation(location);
            OutputMessage.AddByte((byte)effect);
        }
    }
}
