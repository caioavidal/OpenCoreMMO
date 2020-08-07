using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Model.Monsters
{
  

    public struct TargetChance : ITargetChance
    {
        public ushort Interval { get; set; }
        public byte Chance { get; set; }
    }
}
