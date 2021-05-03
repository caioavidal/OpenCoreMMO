using NeoServer.Game.Common.Contracts.Creatures.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Monsters
{
  
    public readonly struct MonsterSummon : IMonsterSummon
    {
        public string Name { get; }
        public uint Interval { get; }
        public byte Chance { get; }
        public byte Max { get; }
    }
}
