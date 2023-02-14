using NeoServer.Game.Common.Contracts.World;

namespace NeoServer.Game.Common.Contracts.Creatures;

public interface INpcFactory
{
    INpc Create(string name, ISpawnPoint spawn = null);
}