using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class CreatureLightPacket : OutgoingPacket
    {
        private readonly IPlayer player;
        public CreatureLightPacket(IPlayer player)
        {
            this.player = player;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.CreatureLight);

            message.AddUInt32(player.CreatureId);
            message.AddByte(player.LightBrightness); // light level
            message.AddByte(player.LightColor); // color
        }
    }
}
