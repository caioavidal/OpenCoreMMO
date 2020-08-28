using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IMonsterDataManager
    {
        void Load(IEnumerable<IMonsterType> monsters);
        bool TryGetMonster(string name, out IMonsterType monster);
    }
}
