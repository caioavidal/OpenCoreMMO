using NeoServer.Game.Contracts.World;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface INpcFactory
    {
        INpc Create(string name, ISpawnPoint spawn = null);
    }
}