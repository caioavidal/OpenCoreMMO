using System;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Creatures.Monsters.Loots;

public record LootItem(Func<IItemType> ItemType, byte Amount, uint Chance, ILootItem[] Items) : ILootItem;