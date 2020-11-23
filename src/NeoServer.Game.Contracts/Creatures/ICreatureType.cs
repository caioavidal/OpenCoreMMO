using NeoServer.Game.Common.Creatures;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICreatureType
    {
        string Name { get; }
        string Description { get; }
        uint MaxHealth { get; }
        ushort Speed { get; }
        IDictionary<LookType, ushort> Look { get; }

    }
}
