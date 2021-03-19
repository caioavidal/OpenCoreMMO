using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;
using System.Linq;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class CreatureMovedDownPacket : OutgoingPacket
    {
        private readonly ICreature creature;
        private readonly Location location;
        private readonly Location toLocation;
        private readonly IMap map;

        public CreatureMovedDownPacket(Location location, Location toLocation, IMap map, ICreature creature)
        {
            this.location = location;
            this.toLocation = toLocation;
            this.map = map;
            this.creature = creature;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.FloorChangeDown);

            var x = (ushort)(location.X - 8);
            var y = (ushort)(location.Y - 6);

            int skip = -1;

            //going from surface to underground
            if (toLocation.Z == 8)
            {

                //todo try to find a better way
                message.AddBytes(map.GetFloorDescription(creature, x, y, toLocation.Z, 18, 14, -1, ref skip).ToArray());
                message.AddBytes(map.GetFloorDescription(creature, x, y, (byte)(toLocation.Z + 1), 18, 14, -2, ref skip).ToArray());
                message.AddBytes(map.GetFloorDescription(creature, x, y, (byte)(toLocation.Z + 2), 18, 14, -3, ref skip).ToArray());

            }
            //going further down
            else if (toLocation.Z > location.Z && toLocation.Z > 8 && toLocation.Z < 14)
            {
                skip = -1;
                message.AddBytes(map.GetFloorDescription(creature, x, y, (byte)(toLocation.Z + 2), 18, 14, -3, ref skip).ToArray());

            }

            if (skip >= 0)
            {
                message.AddByte((byte)skip);
                message.AddByte(0xFF);
            }

            //moving down a floor makes us out of sync

            //east
            message.AddByte((byte)GameOutgoingPacketType.MapSliceEast);
            map.GetDescription(creature, (ushort)(location.X + 9), (ushort)(location.Y - 7), toLocation.Z, toLocation.IsUnderground, 1, 14);

            //south
            message.AddByte((byte)GameOutgoingPacketType.MapSliceSouth);
            map.GetDescription(creature, (ushort)(location.X - 8), (ushort)(location.Y + 7), toLocation.Z, toLocation.IsUnderground, 18, 1);
        }
    }
}
