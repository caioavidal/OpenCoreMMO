using System.Collections.Generic;

namespace NeoServer.Game.Common.Contracts.Creatures;

public interface IMonsterDataManager
{
    void Load(IEnumerable<(string, IMonsterType)> monsters);
    bool TryGetMonster(string name, out IMonsterType monster);
}