using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using System.Buffers;

namespace NeoServer.Game.Effects.Magical
{
    public class SpreadEffect
    {
        /// <summary>
        /// Creates a spread effect based on length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Coordinate[] Create(Direction direction, int length, int spread)
        {
            var pool = ArrayPool<Coordinate>.Shared;
            var points = pool.Rent(length * spread);

            var y = 0;
            var x = 0;

            int count = 0;
            for (int i = 0; i < length; i++)
            {
                var row = i + 1;
                var cols = i < (length / spread) ? 0 : (i + 1) / ((length / spread) + 1);
                for (int c = 0 - cols; c <= 0 + cols; c++)
                {
                    
                    switch (direction)
                    {
                        case Direction.North:
                            points[count++] = new Coordinate(x - c, -row, 0);
                            break;
                        case Direction.East:
                            points[count++] = new Coordinate(+row, y + c, 0);
                            break;
                        case Direction.South:
                            points[count++] = new Coordinate(x + c, +row, 0);
                            break;
                        case Direction.West:
                            points[count++] = new Coordinate(-row, y - c, 0);
                            break;
                        case Direction.None:
                            break;
                        default:
                            break;
                    }
                }
            }

            pool.Return(points);

            return points[0..count];
        }

        public static Coordinate[] Create(Location location, Direction direction, int length, int spread = 1)
        {
            var i = 0;

            var affectedLocations = Create(direction, length, spread);
            var affectedArea = new Coordinate[affectedLocations.Length];

            foreach (var affectedlocation in affectedLocations)
            {
                affectedArea[i++] = location.Translate() + affectedlocation;
            }
            return affectedArea;
        }
    }
}
