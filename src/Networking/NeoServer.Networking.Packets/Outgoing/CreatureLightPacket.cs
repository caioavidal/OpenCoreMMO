using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

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
