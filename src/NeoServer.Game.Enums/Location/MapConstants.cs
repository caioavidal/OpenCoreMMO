namespace NeoServer.Game.Common.Location
{
    public class MapConstants
    {
        public const byte MAX_DISTANCE_MOVE_THING = 11;
        public const byte DefaultMapWindowSizeX = 18;
        public const byte DefaultMapWindowSizeY = 14;

        public const int MaxViewInX = 11;         // min value: maxClientViewportX + 1
        public const int MaxViewInY = 11;         // min value: max
        public const int MaxClientViewportX = 8;
        public const int MaxClientViewportY = 6;
        public const int LimitOfObjectsOnTile = 9;
    }
}
