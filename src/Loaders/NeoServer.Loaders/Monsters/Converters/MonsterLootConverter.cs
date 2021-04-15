using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures;
using System.Collections.Generic;
using System.Linq;
using static NeoServer.Loaders.Monsters.MonsterData;

namespace NeoServer.Loaders.Monsters.Converters
{
    public class MonsterLootConverter
    {
        public static ILoot Convert(MonsterData data, decimal lootRate)
        {
            if (data.Loot is null) return null;

            var items = new List<ILootItem>();

            foreach (var item in Normalize(data.Loot))
            {
                items.Add(ConvertToLootItem(item));
            }

            return new Game.Creatures.Model.Monsters.Loots.Loot(items.ToArray(), null, lootRate);
        }

        private static List<LootData> Normalize(List<LootData> lootData)
        {
            return lootData?.GroupBy(x => x.Id)?.Select(gp => new LootData
            {
                Chance = gp.FirstOrDefault().Chance,
                Id = gp.Key,
                Countmax = gp.Sum(s => byte.TryParse(s.Countmax, out byte amount) ? amount : 0).ToString(),
                Items = Normalize(gp.FirstOrDefault().Items)
            })?.ToList();
        }

        private static ILootItem ConvertToLootItem(LootData item)
        {
            byte.TryParse(item.Countmax, out byte amount);
            ushort.TryParse(item.Id, out ushort id);
            uint.TryParse(item.Chance, out var chance);

            var items = new List<ILootItem>();

            if (item?.Items?.Count > 0)
            {
                foreach (var child in item?.Items)
                {
                    items.Add(ConvertToLootItem(child));
                }
            }

            return new LootItem(id, amount == 0 ? (byte)1 : amount, chance, items.ToArray());
        }
    }
}
