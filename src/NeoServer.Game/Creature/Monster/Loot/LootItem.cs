using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Creature.Monster.Loot;

public record LootItem(Func<IItemType> ItemType, byte Amount, uint Chance, ILootItem[] Items) : ILootItem;