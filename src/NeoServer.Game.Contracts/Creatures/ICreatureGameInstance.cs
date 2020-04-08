using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICreatureGameInstance
    {
        ICreature this[uint id] { get; }

        void Add(ICreature creature);
        IEnumerable<ICreature> All();
        bool TryRemove(uint id);
    }
}