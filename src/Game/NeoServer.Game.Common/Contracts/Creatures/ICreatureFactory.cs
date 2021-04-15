using NeoServer.Game.Contracts.World;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICreatureFactory
    {
        IMonster CreateMonster(string name, ISpawnPoint spawn = null);
        INpc CreateNpc(string name, ISpawnPoint spawn = null);
        IPlayer CreatePlayer(IPlayer playerModel);
    }
}
