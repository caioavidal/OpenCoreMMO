namespace NeoServer.Game.Contracts.Creatures
{
    public interface ILoot
    {
        ILootItem[] Drop();
        ILootItem[] Items { get; }
    }
}
