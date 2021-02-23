using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures.Events
{
    public class CreatureDroppedLootEventHandler: IGameEventHandler
    {
        private readonly IItemFactory itemFactory;
        public CreatureDroppedLootEventHandler(IItemFactory itemFactory)
        {
            this.itemFactory = itemFactory;
        }
        public void Execute(ICombatActor creature, ILoot loot, IEnumerable<ICreature> owners)
        {
            if (creature.Corpse is not IContainer) return;

            CreateLoot(creature, loot);
        }
        private void CreateLoot(ICombatActor creature, ILoot loot)
        {
            if (creature is not IMonster monster) return;
            if (monster.Corpse is not IContainer container) return;

            CreateLootItems(loot.Items, monster.Location, container);
        }

        private void CreateLootItems(ILootItem[] items, Location location, IContainer container)
        {
            foreach (var item in items)
            {
                var attributes = new Dictionary<ItemAttribute, IConvertible>();

                if (item.Amount > 1) attributes.TryAdd(ItemAttribute.Count, item.Amount);

                var itemToDrop = itemFactory.Create(item.ItemId, location, attributes);

                if (itemToDrop is IContainer && item.Items?.Length == 0) continue;

                if (itemToDrop is IContainer c && item.Items?.Length > 0) CreateLootItems(item.Items, location, c);

                container?.AddItem(itemToDrop);
            }
        }
    }
}
