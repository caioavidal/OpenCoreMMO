using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.World.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class MapDescriptionPacket:OutgoingPacket
    {
        public MapDescriptionPacket(IPlayer player,  Map map) : base(false)
        {
            OutputMessage.AddByte((int)GameOutgoingPacketType.MapDescription);
            OutputMessage.AddLocation(player.Location);
            
            OutputMessage.AddBytes(GetMapDescrition(player, map));

        }

        private byte[] GetMapDescrition(IPlayer player, Map map)
        {
            var location = player.Location;
            //c++	GetMapDescription(pos.x - 8, pos.y - 6, pos.z, 18, 14, msg);
            return map.GetDescription(player, (ushort)(location.X - 8), (ushort)(location.Y - 6), location.Z, location.IsUnderground).ToArray();
        }
    }
}
