namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICreatureGameInstance
    {
        ICreature this[uint id] { get; }

        void Add(ICreature creature);
        void Remove(uint id);
    }
}