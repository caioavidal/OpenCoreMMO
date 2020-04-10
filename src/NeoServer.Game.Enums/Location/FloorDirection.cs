using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Enums.Location
{
    public enum FloorChangeDirection : byte
    {
        None,
        Up,
        Down,
        South,
        SouthAlternative,
        EastAlternative,
        North,
        East,
        West
    }
}
