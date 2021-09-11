using System.Collections.Generic;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Common.Effects.Magical
{
    public class WaveEffect
    {
        public static byte[,] AreaWave3 = new byte[,]
        {
            {1, 1, 1},
            {1, 1, 1},
            {0, 3, 0}
        };

        public static byte[,] AreaWave4 = new byte[,]
        {
            {1, 1, 1, 1, 1},
            {0, 1, 1, 1, 0},
            {0, 1, 1, 1, 0},
            {0, 0, 3, 0, 0}
        };

        public static byte[,] AreaWave6 = new byte[,]
        {
            {0, 0, 0, 0, 0},
            {0, 1, 3, 1, 0},
            {0, 0, 0, 0, 0}
        };

        public static byte[,] AreaSquareWave5 = new byte[,]
        {
            {1, 1, 1},
            {1, 1, 1},
            {1, 1, 1},
            {0, 1, 0},
            {0, 3, 0}
        };

        public static byte[,] DiagonalWave4 = new byte[,]
        {
            {0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 8, 8, 7, 8, 8, 0, 0, 0, 0},
            {0, 0, 0, 8, 7, 6, 6, 6, 7, 8, 0, 0, 0},
            {0, 0, 8, 7, 6, 5, 5, 5, 6, 7, 8, 0, 0},
            {0, 8, 7, 6, 5, 4, 4, 4, 5, 6, 7, 8, 0},
            {0, 8, 6, 5, 4, 3, 2, 3, 4, 5, 6, 8, 0},
            {1, 7, 6, 5, 4, 2, 1, 2, 4, 5, 6, 7, 8},
            {0, 8, 6, 5, 4, 3, 2, 3, 4, 5, 6, 8, 0},
            {0, 8, 7, 6, 5, 4, 4, 4, 5, 6, 7, 8, 0},
            {0, 0, 8, 7, 6, 5, 5, 5, 6, 7, 8, 0, 0},
            {0, 0, 0, 8, 7, 6, 6, 6, 7, 8, 0, 0, 0},
            {0, 0, 0, 0, 8, 8, 7, 8, 8, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0}
        };

        public static IEnumerable<Coordinate> Create(byte[,] area)
        {
            var points = new List<Coordinate>();

            for (var row = 0; row < 13; row++)
            for (var col = 0; col < 13; col++)
                if (area[row, col] > 0)
                {
                    var y = row - 6;
                    var x = col - 6;
                    points.Add(new Coordinate(x, y, 0));
                }

            return points;
        }
    }
}