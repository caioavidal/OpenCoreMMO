namespace NeoServer.Game.Effects.Magical
{
    public partial class AreaEffect
    {

        [AreaType("AREA_WAVE3")]
        public static byte[,] Wave3 = new byte[,] {
                                                {1, 1, 1},
                                                {1, 1, 1},
                                                {0, 3, 0}
                                                };
        [AreaType("AREA_WAVE4")]
        public static byte[,] Wave4 = new byte[,] {
            {1, 1, 1, 1, 1},
            {0, 1, 1, 1, 0},
            {0, 1, 1, 1, 0},
            {0, 0, 3, 0, 0}
            };

        [AreaType("AREA_WAVE6")]
        public static byte[,] Wave6 = new byte[,]{
            {0, 0, 0, 0, 0},
            {0, 1, 3, 1, 0},
            {0, 0, 0, 0, 0}
            };

        [AreaType("AREA_SQUAREWAVE5")]
        public static byte[,] SquareWave5 = new byte[,] {
            {1, 1, 1},
            {1, 1, 1},
            {1, 1, 1},
            {0, 1, 0},
            {0, 3, 0}
            };

        [AreaType("AREA_SQUAREWAVE6")]
        public static byte[,] SquareWave6 = new byte[,]  {
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

        [AreaType("AREA_SQUAREWAVE7")]
        public static byte[,] SquareWave7 = new byte[,] {
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
        [AreaType("AREADIAGONAL_WAVE4")]
        public static byte[,] AreaDiagonalWave4 = new byte[,]{
            {0, 0, 0, 0, 1, 0},
            {0, 0, 0, 1, 1, 0},
            {0, 0, 1, 1, 1, 0},
            {0, 1, 1, 1, 1, 0},
            {1, 1, 1, 1, 1, 0},
            {0, 0, 0, 0, 0, 3}
            };

        [AreaType("AREADIAGONAL_SQUAREWAVE5")]
        public static byte[,] AreaDiagonalSquareWave5 = new byte[,] {
            {1, 1, 1, 0, 0},
            {1, 1, 1, 0, 0},
            {1, 1, 1, 0, 0},
            {0, 0, 0, 1, 0},
            {0, 0, 0, 0, 3}
            };
        [AreaType("AREADIAGONAL_WAVE6")]
        public static byte[,] AreaDiagonalWave6 = new byte[,] {
            {0, 0, 1},
            {0, 3, 0},
            {1, 0, 0}
            };

        //Beams
        [AreaType("AREA_BEAM1")]
        public static byte[,] Beam1 = new byte[,] {
            {3}
            };

        [AreaType("AREA_BEAM5")]
        public static byte[,] Beam5 = new byte[,] {
            {1},
            {1},
            {1},
            {1},
            {3}
            };

        [AreaType("AREA_BEAM7")]
        public static byte[,] Beam7 = new byte[,]{
            {1},
            {1},
            {1},
            {1},
            {1},
            {1},
            {3}
            };

        [AreaType("AREA_BEAM8")]
        public static byte[,] Beam8 = new byte[,] {
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
        [AreaType("AREADIAGONAL_BEAM5")]
        public static byte[,] AreaDiagonalBeam5 = new byte[,] {
            {1, 0, 0, 0, 0},
            {0, 1, 0, 0, 0},
            {0, 0, 1, 0, 0},
            {0, 0, 0, 1, 0},
            {0, 0, 0, 0, 3}
            };

        [AreaType("AREADIAGONAL_BEAM7")]
        public static byte[,] AreaDiagonalBeam7 = new byte[,] {
            {1, 0, 0, 0, 0, 0, 0},
            {0, 1, 0, 0, 0, 0, 0},
            {0, 0, 1, 0, 0, 0, 0},
            {0, 0, 0, 1, 0, 0, 0},
            {0, 0, 0, 0, 1, 0, 0},
            {0, 0, 0, 0, 0, 1, 0},
            {0, 0, 0, 0, 0, 0, 3}
            };

        //Circles
        [AreaType("AREA_CIRCLE2X2")]
        public static byte[,] Circle2X2 = new byte[,]{
            {0, 1, 1, 1, 0},
            {1, 1, 1, 1, 1},
            {1, 1, 3, 1, 1},
            {1, 1, 1, 1, 1},
            {0, 1, 1, 1, 0}
            };

        [AreaType("AREA_CIRCLE3X3")]
        public static byte[,] Circle3X3 = new byte[,]{
            {0, 0, 1, 1, 1, 0, 0},
            {0, 1, 1, 1, 1, 1, 0},
            {1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 3, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1},
            {0, 1, 1, 1, 1, 1, 0},
            {0, 0, 1, 1, 1, 0, 0}
            };

        // Crosses
        [AreaType("AREA_CROSS1X1")]
        public static byte[,] Cross1X1 = new byte[,]{
            {0, 1, 0},
            {1, 3, 1},
            {0, 1, 0}
            };

        [AreaType("AREA_CIRCLE5X5")]
        public static byte[,] Circle5X5 = new byte[,] {
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

        [AreaType("AREA_CIRCLE6X6")]
        public static byte[,] Circle6X6 = new byte[,] {
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
        [AreaType("AREA_SQUARE1X1")]
        public static byte[,] Square1X1 = new byte[,] {
            {1, 1, 1},
            {1, 3, 1},
            {1, 1, 1}
            };

        //Walls
        [AreaType("AREA_WALLFIELD")]
        public static byte[,] WallField = new byte[,] {
            {1, 1, 3, 1, 1}
            };

        [AreaType("AREADIAGONAL_WALLFIELD")]
        public static byte[,] AreaDiagonalWallField = new byte[,]{
            {0, 0, 0, 0, 1},
            {0, 0, 0, 1, 1},
            {0, 1, 3, 1, 0},
            {1, 1, 0, 0, 0},
            {1, 0, 0, 0, 0},
            };
    }

}
