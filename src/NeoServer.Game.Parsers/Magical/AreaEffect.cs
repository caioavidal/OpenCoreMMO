using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Effects.Parsers;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Effects.Magical
{
    public class AreaEffect
    {
        public static Dictionary<AreaType, (byte, byte)> originPoints = new Dictionary<AreaType, (byte, byte)>();
        public static byte[,] Circle3x3 = new byte[,]
      {
            {0, 0, 1, 1, 1, 0, 0},
            {0, 1, 1, 1, 1, 1, 0},
            {1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 3, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1},
            {0, 1, 1, 1, 1, 1, 0},
            {0, 0, 1, 1, 1, 0, 0}
      };

        public static Coordinate[] Create(AreaType area)
        {
            var array = AreaTypeParser.Parse(area);
            var origin = FindOriginPoint(area, array);

            var pool = ArrayPool<Coordinate>.Shared;
            var points = pool.Rent(array.Length * array.GetLength(0));

            int count = 0;

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    var value = array[i, y];
                    if (value == 0) continue;

                    points[count++] = new Coordinate(i - origin.Item1, y - origin.Item2, 0);
                }
            }
            pool.Return(points);

            return points[0..count];
        }

        public static Coordinate[] Create(Location location, AreaType area)
        {
            var i = 0;

            var affectedLocations = Create(area);
            var affectedArea = new Coordinate[affectedLocations.Length];

            foreach (var affectedlocation in affectedLocations)
            {
                affectedArea[i++] = location.Translate() + affectedlocation;
            }
            return affectedArea;
        }

        private static (byte, byte) FindOriginPoint(AreaType area, byte[,] array)
        {
            if (originPoints.TryGetValue(area, out var origin)) return origin;

            var length = array.GetLength(0);

            for (int i = 0; i < array.Length; i++)
            {
                for (int y = 0; y < length; y++)
                {
                    var value = array[i, y];
                    if (value == 3)
                    {
                        origin = ((byte)i, (byte)y);
                        originPoints.TryAdd(area, origin);
                        return origin;
                    }
                }
            }

            throw new ArgumentException("origin point not found for area");
        }
    }
}
