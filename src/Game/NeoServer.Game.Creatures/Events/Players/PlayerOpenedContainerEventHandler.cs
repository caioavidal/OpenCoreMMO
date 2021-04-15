using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures.Events.Players
{
    public class PlayerOpenedContainerEventHandler : IGameEventHandler
    {
        public PlayerOpenedContainerEventHandler()
        {
        }

        private readonly IItemFactory itemFactory;
        public PlayerOpenedContainerEventHandler(IItemFactory itemFactory)
        {
            this.itemFactory = itemFactory;
        }
        public void Execute(IPlayer player, byte containerId, IContainer container)
        {
            if (container is not ILootContainer lootContainer || lootContainer.Loot is null) return;
            if (lootContainer.LootCreated) return;

            CreateLoot(lootContainer);
            lootContainer.MarkAsLootCreated();
        }
        private void CreateLoot(ILootContainer lootContainer)
        { 
            CreateLootItems(lootContainer.Loot.Items, lootContainer);
        }

        private void CreateLootItems(ILootItem[] items, IContainer container)
        {
            foreach (var item in items)
            {
                var attributes = new Dictionary<ItemAttribute, IConvertible>();

                if (item.Amount > 1) attributes.TryAdd(ItemAttribute.Count, item.Amount);

                var itemToDrop = itemFactory.Create(item.ItemId, container.Location, attributes);

                if (itemToDrop is IContainer && item.Items?.Length == 0) continue;

                if (itemToDrop is IContainer c && item.Items?.Length > 0) CreateLootItems(item.Items, c);

                container?.AddItem(itemToDrop);
            }
        }
    }
}
