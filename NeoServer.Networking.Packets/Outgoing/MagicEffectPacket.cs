using NeoServer.Server.Model.Creatures;
using NeoServer.Server.Model.World.Structs;
using System;
using System.Collections.Generic;
using System.Text;

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
