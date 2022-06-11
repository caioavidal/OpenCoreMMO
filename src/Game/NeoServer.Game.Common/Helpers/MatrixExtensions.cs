using System;

namespace NeoServer.Game.Common.Helpers;

public static class  MatrixExtensions
{
    public static byte[,] Rotate(this byte[,] area)
    {
        var rotatedArea = new byte[area.GetLength(0), area.GetLength(1)]; 
        
        Buffer.BlockCopy(area, 0,rotatedArea, 0, area.Length);
        
        int rotations = rotatedArea.GetLength(0);

        for (var i = 0; i < rotations / 2; i += 1)
        {
            for (var j = i; j < rotations - i - 1; j += 1)
            {
                int temp = rotatedArea[i, j];
                rotatedArea[i, j] = rotatedArea[j, rotations - 1 - i];
                rotatedArea[j, rotations - 1 - i] = rotatedArea[rotations - 1 - i, rotations - 1 - j];
                rotatedArea[rotations - 1 - i, rotations - 1 - j] = rotatedArea[rotations - 1 - j, i];
                rotatedArea[rotations - 1 - j, i] = (byte)temp;
            }
        }

        return rotatedArea;
    }
}