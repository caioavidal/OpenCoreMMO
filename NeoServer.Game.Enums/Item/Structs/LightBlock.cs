using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Enums
{
    public struct LightBlock
    {
        public byte LightLevel { get; }
        byte LightColor { get; }

        public LightBlock(byte lightLevel, byte lightColor)
        {
            LightLevel = lightLevel;
            LightColor = lightColor;
        }
    }

}
