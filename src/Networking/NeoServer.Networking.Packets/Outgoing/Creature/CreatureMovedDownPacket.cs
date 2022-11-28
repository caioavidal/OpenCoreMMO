using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Creature;

public class CreatureMovedDownPacket : OutgoingPacket
{
    private readonly ICreature creature;
    private readonly Location location;
    private readonly IMap map;
    private readonly Location toLocation;

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

        var skip = -1;
        var x = (ushort)(location.X - MapViewPort.MaxClientViewPortX);
        var y = (ushort)(location.Y - MapViewPort.MaxClientViewPortY);

        //going from surface to underground
        if (toLocation.Z == 8)
        {
            message.AddBytes(map.GetFloorDescription(creature, x, y, toLocation.Z, 18, 14, -1, ref skip).ToArray());

            message.AddBytes(map
                .GetFloorDescription(creature, x, y, (byte)(toLocation.Z + 1), 18, 14, -2, ref skip).ToArray());

            message.AddBytes(map
                .GetFloorDescription(creature, x, y, (byte)(toLocation.Z + 2), 18, 14, -3, ref skip).ToArray());
        }
        //going further down
        else if (toLocation.Z > location.Z && toLocation.Z is > 8 and < 14)
        {
            skip = -1;
            message.AddBytes(map
                .GetFloorDescription(creature, x, y, (byte)(toLocation.Z + 2), 18, 14, -3, ref skip).ToArray());
        }

        if (skip >= 0)
        {
            message.AddByte((byte)skip);
            message.AddByte(0xFF);
        }

        //east
        message.AddByte((byte)GameOutgoingPacketType.MapSliceEast);
        map.GetDescription(creature, (ushort)(location.X + MapViewPort.MaxClientViewPortX + 1),
            (ushort)(location.Y - MapViewPort.MaxClientViewPortY + 1), toLocation.Z, 1);

        //south
        message.AddByte((byte)GameOutgoingPacketType.MapSliceSouth);
        map.GetDescription(creature, (ushort)(location.X - MapViewPort.MaxClientViewPortX),
            (ushort)(location.Y + MapViewPort.MaxClientViewPortY + 1), toLocation.Z, 18, 1);
    }
}