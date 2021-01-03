namespace NeoServer.Game.Common
{
    public readonly ref struct MinMax
    {
        public MinMax(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Min { get; }
        public int Max { get; }

    }
}
