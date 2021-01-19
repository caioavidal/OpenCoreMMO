using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creatures
{
    public record LootItem(ushort ItemId, byte Amount, uint Chance, ILootItem[] Items) : ILootItem;
}
