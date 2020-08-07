using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ITargetChance
    {
        byte Chance { get; set; }
        ushort Interval { get; set; }
    }
}
