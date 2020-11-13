using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Combat.Calculations
{
    public class DistanceHitChanceCalculation
    {
        public static byte CalculateFor2Hands(ushort skill, byte range)
        {
            return (byte)(range switch
            {
                1 => Math.Min(90, (1.2 * skill + 1)),
                2 => Math.Min(90, (3.2 * skill)),
                3 => Math.Min(90, (2 * skill)),
                4 => Math.Min(90, (1.55 * skill)),
                5 => Math.Min(90, (1.2 * skill + 1)),
                6 => Math.Min(90, (int)skill),
                _ => 0
            });
        }
    }
}
