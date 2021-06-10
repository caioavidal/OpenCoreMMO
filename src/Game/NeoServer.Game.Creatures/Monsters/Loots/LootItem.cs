using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Monsters.Loots
{
    public record LootItem(ushort ItemId, byte Amount, uint Chance, ILootItem[] Items) : ILootItem;
}