using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Effects.Parsers;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace NeoServer.Game.Effects.Magical
{
    public partial class AreaEffect
    {
        public static Dictionary<string, (byte, byte)> originPoints = new Dictionary<string, (byte, byte)>();

       
        public static Coordinate[] Create(string areaType)
        {
            var array = AreaTypeParser.Parse(areaType);
            if (array is null) return default;

            var origin = FindOriginPoint(areaType, array);

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

        public static Coordinate[] Create(Location location, string areaType)
        {
            var i = 0;

            var affectedLocations = Create(areaType);
            var affectedArea = new Coordinate[affectedLocations.Length];

            foreach (var affectedlocation in affectedLocations)
            {
                affectedArea[i++] = location.Translate() + affectedlocation;
            }
            return affectedArea;
        }

        private static (byte, byte) FindOriginPoint(string areaType, byte[,] array)
        {
            if (originPoints.TryGetValue(areaType, out var origin)) return origin;

            var length = array.GetLength(0);

            for (int i = 0; i < array.Length; i++)
            {
                for (int y = 0; y < length; y++)
                {
                    var value = array[i, y];
                    if (value == 3)
                    {
                        origin = ((byte)i, (byte)y);
                        originPoints.TryAdd(areaType, origin);
                        return origin;
                    }
                }
            }

            throw new ArgumentException("origin point not found for area");
        }
    }
}
