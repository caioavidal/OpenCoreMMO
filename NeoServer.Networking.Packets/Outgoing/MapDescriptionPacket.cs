using NeoServer.Game.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System.Linq;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class MapDescriptionPacket : OutgoingPacket
    {
        private readonly IPlayer player;
        private readonly IMap map;
        public MapDescriptionPacket(IPlayer player, IMap map)
        {
            this.player = player;
            this.map = map;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((int)GameOutgoingPacketType.MapDescription);
            message.AddLocation(player.Location);

            message.AddBytes(GetMapDescrition(player, map));
        }

        private byte[] GetMapDescrition(IPlayer player, IMap map)
        {
            var location = player.Location;
            //c++	GetMapDescription(pos.x - 8, pos.y - 6, pos.z, 18, 14, msg);
            return map.GetDescription(player, (ushort)(location.X - 8), (ushort)(location.Y - 6), location.Z, location.IsUnderground).ToArray();
        }
    }
}
