namespace NeoServer.Game.Common.Contracts.Creatures
{
    public interface IWalkableMonster
    {
        void Escape(Location.Structs.Location fromLocation);
        bool LookForNewEnemy();
    }
}