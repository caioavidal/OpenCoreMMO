using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using System;
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
        public static IEnumerable<Location> Create(Direction direction, int length)
        {
            var points = new List<Location>();

            var y = 0;
            var x = 0;

            for (int i = 0; i < length; i++)
            {
                switch (direction)
                {
                    case Direction.North:
                        points.Add(new Location(x, --y, 0));
                        break;
                    case Direction.East:
                        points.Add(new Location(++x, y, 0));
                        break;
                    case Direction.South:
                        points.Add(new Location(x, ++y, 0));
                        break;
                    case Direction.West:
                        points.Add(new Location(--x, y, 0));
                        break;
                    case Direction.None:
                        break;
                    default:
                        break;
                }
            }

            return points;
        }
    }
}
