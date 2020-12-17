using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoServer.Game.Effects.Explosion
{
    public class ExplosionEffect
    {

        private static byte[,] _area = new byte[13, 13] {
        {0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 8, 8, 7, 8, 8, 0, 0, 0, 0},
        {0, 0, 0, 8, 7, 6, 6, 6, 7, 8, 0, 0, 0},
        {0, 0, 8, 7, 6, 5, 5, 5, 6, 7, 8, 0, 0},
        {0, 8, 7, 6, 5, 4, 4, 4, 5, 6, 7, 8, 0},
        {0, 8, 6, 5, 4, 3, 2, 3, 4, 5, 6, 8, 0},
        {8, 7, 6, 5, 4, 2, 1, 2, 4, 5, 6, 7, 8},
        {0, 8, 6, 5, 4, 3, 2, 3, 4, 5, 6, 8, 0},
        {0, 8, 7, 6, 5, 4, 4, 4, 5, 6, 7, 8, 0},
        {0, 0, 8, 7, 6, 5, 5, 5, 6, 7, 8, 0, 0},
        {0, 0, 0, 8, 7, 6, 6, 6, 7, 8, 0, 0, 0},
        {0, 0, 0, 0, 8, 8, 7, 8, 8, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0}
    };

        public static IEnumerable<Coordinate> Create(int radius)
        {
            var points = new List<Coordinate>();

            for (int row = 0; row < 13; row++)
            {
                for (int col = 0; col < 13; col++)
                {
                    if (_area[row, col] > 0 && _area[row, col] <= radius)
                    {
                        var y = row - 6;
                        var x = col - 6;
                        points.Add(new Coordinate(x, y, 0));
                    }
                }
            }

            return points;
        }

        public static IEnumerable<Coordinate> Create(Location location, int radius)
        {
            var i = 0;

            var affectedLocations = Create(radius);
            var affectedArea = new Coordinate[affectedLocations.Count()];

            foreach (var affectedLocation in affectedLocations)
            {
                affectedArea[i++] = location.Translate() + affectedLocation;
            }
            return affectedArea;
        }
    }

}
