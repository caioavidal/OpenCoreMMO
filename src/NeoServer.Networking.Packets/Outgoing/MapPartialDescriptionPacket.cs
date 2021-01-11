using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Contracts.Network;
using System.Linq;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class MapPartialDescriptionPacket : OutgoingPacket
    {
        private readonly IThing thing;
        private readonly Location toLocation;
        private readonly Location fromLocation;
        private readonly IMap map;
        private readonly Direction direction;
        public MapPartialDescriptionPacket(IThing thing, Location fromLocation, Location toLocation, Direction direction, IMap map)
        {
            this.thing = thing;
            this.toLocation = toLocation;
            this.map = map;
            this.direction = direction;
            this.fromLocation = fromLocation;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            WriteDirectionMapSlice(message, direction);
        }

        private void WriteDirectionMapSlice(INetworkMessage message, Direction direction)
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
                case Direction.NorthEast:
                    WriteDirectionMapSlice(message, Direction.North);
                    WriteDirectionMapSlice(message, Direction.East);
                    return;
                case Direction.NorthWest:
                    WriteDirectionMapSlice(message, Direction.North);
                    WriteDirectionMapSlice(message, Direction.West);
                    return;
                case Direction.SouthWest:
                    WriteDirectionMapSlice(message, Direction.South);
                    WriteDirectionMapSlice(message, Direction.West);
                    return;
                case Direction.SouthEast:
                    WriteDirectionMapSlice(message, Direction.South);
                    WriteDirectionMapSlice(message, Direction.East);
                    return;

                default:
                    throw new System.ArgumentException("No direction received");
            }

            message.AddBytes(GetDescription(thing, fromLocation, toLocation, map, direction));
        }

        private byte[] GetDescription(Game.Contracts.Items.IThing thing, Location fromLocation, Location toLocation, IMap map, Direction direction)
        {
            var newLocation = toLocation;

            byte width = 1;
            byte height = 1;

            switch (direction)
            {
                case Direction.East:
                    newLocation.X = (ushort)(toLocation.X + 9);
                    newLocation.Y = (ushort)(toLocation.Y - 6);
                    height = 14;
                    break;
                case Direction.West:
                    newLocation.X = (ushort)(toLocation.X - 8);
                    newLocation.Y = (ushort)(toLocation.Y - 6);
                    height = 14;
                    break;
                case Direction.North:
                    newLocation.X = (ushort)(fromLocation.X - 8);
                    newLocation.Y = (ushort)(toLocation.Y - 6);
                    width = 18;
                    break;
                case Direction.South:
                    newLocation.X = (ushort)(fromLocation.X - 8);
                    newLocation.Y = (ushort)(toLocation.Y + 7);
                    width = 18;
                    break;
            }
            return
                map.GetDescription(thing, newLocation.X,
                newLocation.Y,
                toLocation.Z,
                thing.Location.IsUnderground, width, height).ToArray();
        }
    }
}