using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items.Types;

namespace NeoServer.Game.Common.Contracts.Items
{
    public interface ILootContainer : IContainer
    {
        ILoot Loot { get; }
        bool LootCreated { get; }

        bool CanBeOpenedBy(IPlayer player);
        void MarkAsLootCreated();
    }
}