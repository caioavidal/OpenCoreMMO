using System;
using System.Buffers;
using System.Collections.Generic;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Common.Effects.Magical;

public partial class AreaEffect
{
    public static Dictionary<string, (byte, byte)> OriginPoints = new();

    public static Coordinate[] Create(string areaType, byte[,] areaTemplate)
    {
        var array = areaTemplate;
        if (array is null) return default;

        var origin = FindOriginPoint(areaType, array);

        var pool = ArrayPool<Coordinate>.Shared;
        var points = pool.Rent(array.Length * array.GetLength(0));

        var count = 0;

        for (var i = 0; i < array.GetLength(0); i++)
        for (var y = 0; y < array.GetLength(1); y++)
        {
            var value = array[i, y];
            if (value == 0) continue;

            points[count++] = new Coordinate(i - origin.Item1, y - origin.Item2, 0);
        }

        pool.Return(points);

        return points[..count];
    }

    public static Coordinate[] Create(Location.Structs.Location location, string areaType, byte[,] areaTemplate)
    {
        var i = 0;

        var affectedLocations = Create(areaType, areaTemplate);
        var affectedArea = new Coordinate[affectedLocations.Length];

        foreach (var affectedlocation in affectedLocations)
            affectedArea[i++] = location.Translate() + affectedlocation;
        return affectedArea;
    }

    private static (byte, byte) FindOriginPoint(string areaType, byte[,] array)
    {
        if (OriginPoints.TryGetValue(areaType, out var origin)) return origin;

        var length = array.GetLength(0);

        for (var i = 0; i < array.Length; i++)
        for (var y = 0; y < length; y++)
        {
            var value = array[i, y];
            if (value == 3)
            {
                origin = ((byte)i, (byte)y);
                OriginPoints.TryAdd(areaType, origin);
                return origin;
            }
        }

        throw new ArgumentException("origin point not found for area");
    }
}