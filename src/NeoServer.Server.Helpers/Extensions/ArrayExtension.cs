using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NeoServer.Server.Helpers.Extensions
{
    public static class ArrayExtension
    {

        public static Location[] Random(this Location[] values)
        {
            
            for (int i = 0; i < Math.Abs(values.Length / 2); i++)
            {
                var index = ServerRandom.Random.Next(minValue: 0, maxValue:values.Length - 1);
                var v = values[i];
                values[i] = values[index];
                values[index] = v;
                i++;
            }

            return values;
        }
    }
}

