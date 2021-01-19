using NeoServer.Game.Common.Players;
using System;
using System.Buffers;

namespace NeoServer.Game.Common.Location.Structs
{
    public struct Location : IEquatable<Location>
    {

        public Location(ushort x, ushort y, byte z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Location(Coordinate coordinate) : this()
        {
            X = (ushort)coordinate.X;
            Y = (ushort)coordinate.Y;
            Z = (byte)coordinate.Z;
        }
        public Location(Slot slot) : this()
        {
            X = 0xFFFF;
            Y = (byte)slot;
        }

        public void Update(ushort x, ushort y, byte z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public ushort X { get; set; }

        public ushort Y { get; set; }

        public byte Z { get; set; }

        public bool IsUnderground => Z > 7;
        public bool IsAboveSurface => Z < 7;
        public bool IsSurface => Z == 7;

        public int GetOffSetZ(Location location) => Z - location.Z;

        public LocationType Type
        {
            get
            {
                if (X != 0xFFFF)
                {
                    return LocationType.Ground;
                }

                if ((Y & 0x40) != 0)
                {
                    return LocationType.Container;
                }

                return LocationType.Slot;
            }
        }

        public static Coordinate operator +(Location location1, Location location2)
        {
            return new Coordinate(
            x: (location1.X + location2.X),
            y: (location1.Y + location2.Y),
            z: (sbyte)(location1.Z + location2.Z)
            );
        }

        public static Coordinate operator -(Location location1, Location location2)
        {
            return new Coordinate
            (
                x: location2.X - location1.X,
                y: location2.Y - location1.Y,
                z: (sbyte)(location2.Z - location1.Z)
            );
        }

        public Slot Slot => (Slot)Convert.ToByte(Y);

       // public byte Container => Convert.ToByte(Y - 0x40);
        public byte ContainerId => Convert.ToByte(Y & 0x0F);
    
        public sbyte ContainerSlot
        {
            get
            {
                return Convert.ToSByte(Z);
            }
        }

        public int MaxValueIn2D => Math.Max(Math.Abs(X), Math.Abs(Y));

        public int MaxValueIn3D => Math.Max(MaxValueIn2D, Math.Abs(Z));

        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}]";
        }

        public bool Equals(Location obj)
        {
            return this == obj;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public static bool operator ==(Location origin, Location targetLocation)
        {
            try
            {
                return origin.X == targetLocation.X && origin.Y == targetLocation.Y && origin.Z == targetLocation.Z;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public static bool operator !=(Location origin, Location targetLocation)
        {
            try
            {
                return origin.X != targetLocation.X || origin.Y != targetLocation.Y || origin.Z != targetLocation.Z;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public static bool operator >(Location first, Location second)
        {
            try
            {
                return first.X > second.X || first.Y > second.Y || first.Z > second.Z;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public static bool operator <(Location first, Location second)
        {
            try
            {
                return first.X < second.X || first.Y < second.Y || first.Z < second.Z;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public static long[] GetOffsetBetween(Location origin, Location targetLocation)
        {
            return new[] {
                (long)origin.X - targetLocation.X,
                (long)origin.Y - targetLocation.Y,
                (long)origin.Z - targetLocation.Z
            };
        }

        public bool IsDiagonalMovement(Location targetLocation)
        {
            return DirectionTo(targetLocation, true) == Direction.NorthEast ||
                DirectionTo(targetLocation, true) == Direction.NorthWest ||
                DirectionTo(targetLocation, true) == Direction.SouthEast ||
                DirectionTo(targetLocation, true) == Direction.SouthWest;
        }

        public Location GetNextLocation(Direction direction)
        {
            var toLocation = this;

            switch (direction)
            {
                case Direction.East:
                    toLocation.X += 1;
                    break;
                case Direction.West:
                    toLocation.X -= 1;
                    break;
                case Direction.North:
                    toLocation.Y -= 1;
                    break;
                case Direction.South:
                    toLocation.Y += 1;
                    break;
                case Direction.NorthEast:
                    toLocation.X += 1;
                    toLocation.Y -= 1;
                    break;
                case Direction.NorthWest:
                    toLocation.X -= 1;
                    toLocation.Y -= 1;
                    break;
                case Direction.SouthEast:
                    toLocation.X += 1;
                    toLocation.Y += 1;
                    break;
                case Direction.SouthWest:
                    toLocation.X -= 1;
                    toLocation.Y += 1;
                    break;

            }

            return toLocation;
        }

        public Direction DirectionTo(Location targetLocation, bool returnDiagonals = false)
        {
            var locationDiff = this - targetLocation;

            if (!returnDiagonals)
            {
                var directionX = Direction.None;
                var directionY = Direction.None;

                if (locationDiff.X < 0) directionX = Direction.West;
                if (locationDiff.X > 0) directionX = Direction.East;
                if (locationDiff.Y > 0) directionY = Direction.South;
                if (locationDiff.Y < 0) directionY = Direction.North;

                if (directionX != Direction.None && directionY == Direction.None) return directionX;
                if (directionY != Direction.None && directionX == Direction.None) return directionY;

                if (directionX == Direction.None && directionY == Direction.None) return Direction.None;

                return GetSqmDistanceX(targetLocation) >= GetSqmDistanceY(targetLocation) ? directionX : directionY;

            }

            if (locationDiff.X < 0)
            {
                if (locationDiff.Y < 0)
                {
                    return Direction.NorthWest;
                }

                return locationDiff.Y > 0 ? Direction.SouthWest : Direction.West;
            }

            if (locationDiff.X > 0)
            {
                if (locationDiff.Y < 0)
                {
                    return Direction.NorthEast;
                }

                return locationDiff.Y > 0 ? Direction.SouthEast : Direction.East;
            }

            return locationDiff.Y < 0 ? Direction.North : Direction.South;
        }

        public ushort GetSqmDistance(Location dest)
        {

            var offset = GetOffsetBetween(this, dest);

            return (ushort)(Math.Abs(offset[0]) + Math.Abs(offset[1]));
        }
        public int GetMaxSqmDistance(Location dest) => Math.Max(GetSqmDistanceX(dest), GetSqmDistanceY(dest));
        public int GetSumSqmDistance(Location dest) => GetSqmDistanceX(dest) + GetSqmDistanceY(dest);

        public int GetSqmDistanceX(Location dest) => (ushort)Math.Abs(X - dest.X);
        public int GetSqmDistanceY(Location dest) => (ushort)Math.Abs(Y - dest.Y);

        public Location[] Neighbours
        {
            get
            {
                var pool = ArrayPool<Location>.Shared;
                var locations = pool.Rent(8);

                locations[0] = (Translate() + new Coordinate(-1, 0, 0)).Location;
                locations[1] = (Translate() + new Coordinate(0, 1, 0)).Location;
                locations[2] = (Translate() + new Coordinate(1, 0, 0)).Location;
                locations[3] = (Translate() + new Coordinate(0, -1, 0)).Location;
                locations[4] = (Translate() + new Coordinate(-1, -1, 0)).Location;
                locations[5] = (Translate() + new Coordinate(1, -1, 0)).Location;
                locations[6] = (Translate() + new Coordinate(1, 1, 0)).Location;
                locations[7] = (Translate() + new Coordinate(-1, 1, 0)).Location;

                pool.Return(locations);

                return locations[0..8];
            }
        }

        public static Location Zero => new Location(0, 0, 0);
        public static Location Inventory(Slot slot) => new Location(0xFFFF, (byte)slot, 0);
        public static Location Container(int id, byte containerSlot) => new Location(0xFFFF, (ushort)(id + 64), containerSlot);

        /// <summary>
        /// Check whether location is 1 sqm next to dest
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsNextTo(Location dest) => Math.Abs(X - dest.X) <= 1 && Math.Abs(Y - dest.Y) <= 1 && Z == dest.Z;

        public override bool Equals(object obj)
        {
            return obj is Location && Equals(obj);
        }

        public Coordinate Translate() => new Coordinate(X, Y, (sbyte)Z);
    }
}
