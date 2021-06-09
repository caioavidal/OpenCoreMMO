using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ILoot
    {
        ILootItem[] Items { get; }
        HashSet<ICreature> Owners { get; }
        ILootItem[] Drop();
    }
}