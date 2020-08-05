using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures
{
    public class MonsterFactory: IMonsterFactory
    {
        private readonly IMonsterDataManager _monsterManager;

        private uint _id;

        private object _lock = new object();

        public MonsterFactory(IMonsterDataManager monsterManager)
        {
            _monsterManager = monsterManager;
        }
        public IMonster Create(string name)
        {
            var result = _monsterManager.TryGetMonster(name, out IMonsterType monsterType);
            if (result == false)
            {
                throw new KeyNotFoundException($"Given monster name: {name} is not loaded");
            }

            lock (_lock)
            {
                return new Monster(++_id, monsterType);
            }
        }
    }
}
