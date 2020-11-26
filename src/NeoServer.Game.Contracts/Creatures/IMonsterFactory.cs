using NeoServer.Game.Contracts.World;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IMonsterFactory
    {
        IMonster Create(string name, ISpawnPoint spawn = null);
    }
}
