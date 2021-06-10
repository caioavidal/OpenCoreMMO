namespace NeoServer.Game.Common.Contracts.Creatures
{
    public interface ILootItem
    {
        ushort ItemId { get; }
        byte Amount { get; }
        uint Chance { get; }
        ILootItem[] Items { get; }
    }
}