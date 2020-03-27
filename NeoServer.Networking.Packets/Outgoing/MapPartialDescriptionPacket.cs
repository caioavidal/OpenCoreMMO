using System.Linq;
using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players;
using NeoServer.Server.World.Map;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class MapPartialDescriptionPacket : OutgoingPacket
    {
        private readonly IThing thing;
        private readonly Location toLocation;
        private readonly IMap map;
        private readonly Direction direction;
        public MapPartialDescriptionPacket(IThing thing, Location toLocation, Direction direction, IMap map)
        {
            this.thing = thing;
            this.toLocation = toLocation;
            this.map = map;
            this.direction = direction;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            switch (direction)
            {
                case Direction.East:
                    message.AddByte((byte)GameOutgoingPacketType.MapSliceEast);
                    break;
                case Direction.West:
                    message.AddByte((byte)GameOutgoingPacketType.MapSliceWest);
                    break;
                case Direction.North:
                    message.AddByte((byte)GameOutgoingPacketType.MapSliceNorth);
                    break;
                case Direction.South:
                    message.AddByte((byte)GameOutgoingPacketType.MapSliceSouth);
                    break;
                    //default:
                    //throw new InvalidArgumentExpcetion();
            }

            message.AddBytes(GetDescription(thing, toLocation, map));
        }

        private byte[] GetDescription(IThing thing, Location toLocation, IMap map)
        {
            var newLocation = toLocation;

            byte width = 1;
            byte height = 1;

            switch (direction)
            {
                case Direction.East:
                    newLocation.X = toLocation.X + 9;
                    newLocation.Y = toLocation.Y - 6;
                    height = 14;
                    break;
                case Direction.West:
                    newLocation.X = toLocation.X - 8;
                    newLocation.Y = toLocation.Y - 6;
                    height = 14;
                    break;
                case Direction.North:
                    newLocation.X = toLocation.X - 8;
                    newLocation.Y = toLocation.Y - 6;
                    width = 18;
                    break;
                case Direction.South:
                    newLocation.X = toLocation.X - 8;
                    newLocation.Y = toLocation.Y + 7;
                    width = 18;
                    break;
            }

            return
                map.GetDescription(thing, (ushort)(newLocation.X),
                (ushort)(newLocation.Y),
                toLocation.Z,
                thing.Location.IsUnderground, width, height).ToArray();
        }
    }
}