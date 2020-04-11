using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICreatureGameInstance
    {
        bool TryGetCreature(uint id, out ICreature creature);
        void Add(ICreature creature);
        IEnumerable<ICreature> All();
        bool TryRemove(uint id);
    }
}