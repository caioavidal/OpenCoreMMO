using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Contracts.Creatures.Monsters
{
    public interface IMonsterSummon
    {
        byte Chance { get; }
        uint Interval { get; }
        byte Max { get; }
        string Name { get; }
    }
}
