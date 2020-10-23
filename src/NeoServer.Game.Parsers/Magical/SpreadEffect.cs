using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Effects.Magical
{
    public class SpreadEffect
    {
        /// <summary>
        /// Creates a spread effect based on length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Location[] Create(Direction direction, int length)
        {
            var pool = ArrayPool<Location>.Shared;
            var points = pool.Rent(length);

            var y = 0;
            var x = 0;

            for (int i = 0; i < length; i++)
            {
                switch (direction)
                {
                    case Direction.North:
                        points[i] = new Location(x, --y, 0);
                        break;
                    case Direction.East:
                        points[i] = new Location(++x, y, 0);
                        break;
                    case Direction.South:
                        points[i] = new Location(x, ++y, 0);
                        break;
                    case Direction.West:
                        points[i] = new Location(--x, y, 0);
                        break;
                    case Direction.None:
                        break;
                    default:
                        break;
                }
            }

            pool.Return(points);

            return points[0..length];
        }
    }
}
