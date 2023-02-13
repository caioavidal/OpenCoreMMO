using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Game.Creatures.Monster.Loot;

public record Loot(ILootItem[] Items, HashSet<ICreature> Owners, decimal LootRate = 1) : ILoot
{
    public virtual ILootItem[] Drop()
    {
        return Drop(Items);
    }

    public ILootItem[] Drop(ILootItem[] items)
    {
        var drop = new List<ILootItem>(Items.Length);

        foreach (var item in items)
        {
            var random = GameRandom.Random.Next(1, maxValue: 100_000) / LootRate;

            if (item.Chance < random) continue;

            var itemToDrop = item;

            ILootItem[] childrenItems = null;
            if (item?.Items?.Length > 0) childrenItems = Drop(item.Items);

            if (item?.Items?.Length > 0 && childrenItems?.Length == 0) continue;

            var amount = item.Amount;
            if (amount > 1) amount = (byte)(random % item.Amount + 1);

            if (amount == 0) continue;

            itemToDrop = new LootItem(itemToDrop.ItemType, Math.Min(amount, (byte)100), itemToDrop.Chance,
                childrenItems);
            drop.Add(itemToDrop);
        }

        return drop.ToArray();
    }
}