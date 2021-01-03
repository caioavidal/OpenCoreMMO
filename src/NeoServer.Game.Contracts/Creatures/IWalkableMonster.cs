using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IWalkableMonster
    {
        void Escape(Location fromLocation);
        bool LookForNewEnemy();
    }
}
