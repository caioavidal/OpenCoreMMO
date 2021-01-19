namespace NeoServer.Game.Common
{
    public struct LightBlock
    {
        public byte LightLevel { get; }
        public byte LightColor { get; }

        public LightBlock(byte lightLevel, byte lightColor)
        {
            LightLevel = lightLevel;
            LightColor = lightColor;
        }
    }

}
