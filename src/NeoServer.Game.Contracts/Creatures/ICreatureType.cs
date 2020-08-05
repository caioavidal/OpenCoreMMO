using NeoServer.Game.Enums.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICreatureType
    {
        string Name { get; set; }
        string Description { get; set; }
        uint MaxHealth { get; set; }
        ushort Speed { get; set; }
        IDictionary<LookType, ushort> Look { get; set; }

    }
}
