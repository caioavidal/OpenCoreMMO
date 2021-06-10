namespace NeoServer.Game.Common.Effects.Magical
{
    public partial class AreaEffect
    {
        [AreaType("AREA_WAVE3")] public static byte[,] Wave3 =
        {
            {1, 1, 1},
            {1, 1, 1},
            {0, 3, 0}
        };

        [AreaType("AREA_WAVE4")] public static byte[,] Wave4 =
        {
            {1, 1, 1, 1, 1},
            {0, 1, 1, 1, 0},
            {0, 1, 1, 1, 0},
            {0, 0, 3, 0, 0}
        };

        [AreaType("AREA_WAVE6")] public static byte[,] Wave6 =
        {
            {0, 0, 0, 0, 0},
            {0, 1, 3, 1, 0},
            {0, 0, 0, 0, 0}
        };

        [AreaType("AREA_SQUAREWAVE5")] public static byte[,] SquareWave5 =
        {
            {1, 1, 1},
            {1, 1, 1},
            {1, 1, 1},
            {0, 1, 0},
            {0, 3, 0}
        };

        [AreaType("AREA_SQUAREWAVE6")] public static byte[,] SquareWave6 =
        {
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
            {0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0},
            {0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0},
            {0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0},
            {0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0},
            {0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0},
            {0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0},
            {0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0}
        };

        [AreaType("AREA_SQUAREWAVE7")] public static byte[,] SquareWave7 =
        {
            {0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0},
            {0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0},
            {0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0},
            {0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0},
            {0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0},
            {0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0},
            {0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0},
            {0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0},
            {0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0}
        };

        //Diagonal waves
        [AreaType("AREADIAGONAL_WAVE4")] public static byte[,] AreaDiagonalWave4 =
        {
            {0, 0, 0, 0, 1, 0},
            {0, 0, 0, 1, 1, 0},
            {0, 0, 1, 1, 1, 0},
            {0, 1, 1, 1, 1, 0},
            {1, 1, 1, 1, 1, 0},
            {0, 0, 0, 0, 0, 3}
        };

        [AreaType("AREADIAGONAL_SQUAREWAVE5")] public static byte[,] AreaDiagonalSquareWave5 =
        {
            {1, 1, 1, 0, 0},
            {1, 1, 1, 0, 0},
            {1, 1, 1, 0, 0},
            {0, 0, 0, 1, 0},
            {0, 0, 0, 0, 3}
        };

        [AreaType("AREADIAGONAL_WAVE6")] public static byte[,] AreaDiagonalWave6 =
        {
            {0, 0, 1},
            {0, 3, 0},
            {1, 0, 0}
        };

        //Beams
        [AreaType("AREA_BEAM1")] public static byte[,] Beam1 =
        {
            {3}
        };

        [AreaType("AREA_BEAM5")] public static byte[,] Beam5 =
        {
            {1},
            {1},
            {1},
            {1},
            {3}
        };

        [AreaType("AREA_BEAM7")] public static byte[,] Beam7 =
        {
            {1},
            {1},
            {1},
            {1},
            {1},
            {1},
            {3}
        };

        [AreaType("AREA_BEAM8")] public static byte[,] Beam8 =
        {
            {1},
            {1},
            {1},
            {1},
            {1},
            {1},
            {1},
            {3}
        };

        //Diagonal Beams
        [AreaType("AREADIAGONAL_BEAM5")] public static byte[,] AreaDiagonalBeam5 =
        {
            {1, 0, 0, 0, 0},
            {0, 1, 0, 0, 0},
            {0, 0, 1, 0, 0},
            {0, 0, 0, 1, 0},
            {0, 0, 0, 0, 3}
        };

        [AreaType("AREADIAGONAL_BEAM7")] public static byte[,] AreaDiagonalBeam7 =
        {
            {1, 0, 0, 0, 0, 0, 0},
            {0, 1, 0, 0, 0, 0, 0},
            {0, 0, 1, 0, 0, 0, 0},
            {0, 0, 0, 1, 0, 0, 0},
            {0, 0, 0, 0, 1, 0, 0},
            {0, 0, 0, 0, 0, 1, 0},
            {0, 0, 0, 0, 0, 0, 3}
        };

        //Circles
        [AreaType("AREA_CIRCLE2X2")] public static byte[,] Circle2X2 =
        {
            {0, 1, 1, 1, 0},
            {1, 1, 1, 1, 1},
            {1, 1, 3, 1, 1},
            {1, 1, 1, 1, 1},
            {0, 1, 1, 1, 0}
        };

        [AreaType("AREA_CIRCLE3X3")] public static byte[,] Circle3X3 =
        {
            {0, 0, 1, 1, 1, 0, 0},
            {0, 1, 1, 1, 1, 1, 0},
            {1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 3, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1},
            {0, 1, 1, 1, 1, 1, 0},
            {0, 0, 1, 1, 1, 0, 0}
        };

        // Crosses
        [AreaType("AREA_CROSS1X1")] public static byte[,] Cross1X1 =
        {
            {0, 1, 0},
            {1, 3, 1},
            {0, 1, 0}
        };

        [AreaType("AREA_CIRCLE5X5")] public static byte[,] Circle5X5 =
        {
            {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
            {0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0},
            {0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0},
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
            {1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1},
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
            {0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0},
            {0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0},
            {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0}
        };

        [AreaType("AREA_CIRCLE6X6")] public static byte[,] Circle6X6 =
        {
            {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0},
            {0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0},
            {0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0},
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
            {1, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1},
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
            {0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0},
            {0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0},
            {0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0}
        };

        //Squares
        [AreaType("AREA_SQUARE1X1")] public static byte[,] Square1X1 =
        {
            {1, 1, 1},
            {1, 3, 1},
            {1, 1, 1}
        };

        //Walls
        [AreaType("AREA_WALLFIELD")] public static byte[,] WallField =
        {
            {1, 1, 3, 1, 1}
        };

        [AreaType("AREADIAGONAL_WALLFIELD")] public static byte[,] AreaDiagonalWallField =
        {
            {0, 0, 0, 0, 1},
            {0, 0, 0, 1, 1},
            {0, 1, 3, 1, 0},
            {1, 1, 0, 0, 0},
            {1, 0, 0, 0, 0}
        };
    }
}