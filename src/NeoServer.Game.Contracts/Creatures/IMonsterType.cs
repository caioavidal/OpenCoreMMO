using NeoServer.Game.Enums.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IMonsterType : ICreatureType
    {
        ushort Armor { get; set; }
        ushort Defence { get; set; }
    }
}
