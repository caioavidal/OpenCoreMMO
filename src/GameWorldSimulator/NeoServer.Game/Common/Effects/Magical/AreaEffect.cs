using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Common.Effects.Magical;

public static partial class AreaEffect
{
    private const byte ORIGIN = 3;
    private const byte BREAKING_ROW = byte.MaxValue;

    public static void Create(Location.Structs.Location location, Span<byte> areaTemplate, ref Span<Coordinate> result)
    {
        if (areaTemplate.Length == 0) return;

        var origin = ScanOrigin(areaTemplate);

        var currentRow = 0;
        var currentColumn = 0;
        var index = 0;

        foreach (var point in areaTemplate)
        {
            if (point == BREAKING_ROW)
            {
                currentRow++;
                currentColumn = 0;
                continue;
            }

            var x = location.X + (currentColumn - origin.Column);
            var y = location.Y + (currentRow - origin.Row);
            result[index++] = new Coordinate(x, y, (sbyte)location.Z);

            currentColumn++;
        }
    }

    public static Coordinate[] Create(Location.Structs.Location location, Span<byte> areaTemplate)
    {
        if (areaTemplate.Length == 0) return [];

        var origin = ScanOrigin(areaTemplate);

        var currentRow = 0;
        var currentColumn = 0;
        var index = 0;

        var coordinates = new Coordinate[areaTemplate.Length];

        foreach (var point in areaTemplate)
        {
            if (point == BREAKING_ROW)
            {
                currentRow++;
                currentColumn = 0;
                continue;
            }

            var x = location.X + (currentColumn - origin.Column);
            var y = location.Y + (currentRow - origin.Row);
            coordinates[index++] = new Coordinate(x, y, (sbyte)location.Z);

            currentColumn++;
        }

        return coordinates;
    }

    public static Coordinate[] Create(Location.Structs.Location location, byte[,] areaTemplate)
    {
        if (areaTemplate.Length == 0) return [];

        var origin = ScanOrigin(areaTemplate);

        var coordinates = new Coordinate[areaTemplate.Length];

        var index = 0;
        for (int row = 0; row < areaTemplate.GetLength(0); row++)
        {
            for (int column = 0; column < areaTemplate.GetLength(1); column++)
            {
                var x = location.X + (column - origin.Column);
                var y = location.Y + (row - origin.Row);
                coordinates[index++] = new Coordinate(x, y, (sbyte)location.Z);
            }
        }

        return coordinates;
    }

    private static (int Row, int Column) ScanOrigin(byte[,] areaTemplate)
    {
        (int Row, int Column) origin = default;

        for (int row = 0; row < areaTemplate.GetLength(0); row++)
        {
            for (int column = 0; column < areaTemplate.GetLength(1); column++)
            {
                if (areaTemplate[row, column] == ORIGIN)
                {
                    origin = (row, column);
                }
            }
        }

        return origin;
    }

    private static (int Row, int Column) ScanOrigin(Span<byte> areaTemplate)
    {
        var currentRow = 0;
        var currentColumn = 0;
        (int Row, int Column) origin = (0, 0);

        foreach (var point in areaTemplate)
        {
            if (point == BREAKING_ROW)
            {
                currentRow++;
                currentColumn = 0;
                continue;
            }

            if (point == ORIGIN)
            {
                origin = (currentRow, currentColumn);
                break;
            }

            currentColumn++;
        }

        return origin;
    }
}