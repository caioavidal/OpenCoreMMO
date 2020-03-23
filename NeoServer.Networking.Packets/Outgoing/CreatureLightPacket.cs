using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class CreatureLightPacket: OutgoingPacket
    {
        public CreatureLightPacket(IPlayer player) : base(false)
        {
            OutputMessage.AddByte((byte)GameOutgoingPacketType.CreatureLight);

            OutputMessage.AddUInt32(player.CreatureId);
            OutputMessage.AddByte(player.LightBrightness); // light level
            OutputMessage.AddByte(player.LightColor); // color
        }
    }
}
