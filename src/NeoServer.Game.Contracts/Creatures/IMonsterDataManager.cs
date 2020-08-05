using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IMonsterDataManager
    {
        void Load(IEnumerable<IMonsterType> monsters);
        bool TryGetMonster(string name, out IMonsterType monster);
    }
}
