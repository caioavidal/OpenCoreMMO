using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures;
using System.Collections.Generic;
using static NeoServer.Loaders.Monsters.MonsterData;

namespace NeoServer.Loaders.Monsters.Converters
{
    public class MonsterLootConverter
    {
        public static ILoot Convert(MonsterData data, decimal lootRate)
        {
            if (data.Loot is null) return null;

            var items = new List<ILootItem>();

            foreach (var item in data.Loot)
            {
                items.Add(ConvertToLootItem(item));
            }

            return new Game.Creatures.Model.Monsters.Loots.Loot(items.ToArray(), lootRate);
        }

        private static ILootItem ConvertToLootItem(LootData item)
        {
            byte.TryParse(item.Countmax, out byte amount);
            ushort.TryParse(item.Id, out ushort id); 
            uint.TryParse(item.Chance, out var chance);

            var items = new List<ILootItem>();

            if(item?.Items?.Count > 0)
            {
                foreach (var child in item?.Items)
                {
                    items.Add(ConvertToLootItem(child));
                }
            }

            return new LootItem(id, amount ==0 ? 1 : amount, chance, items.ToArray());
        }
    }
}
