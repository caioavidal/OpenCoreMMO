using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures
{
    public class MonsterFactory : IMonsterFactory
    {
        private readonly IMonsterDataManager _monsterManager;
        private readonly CreatureInjuredEventHandler _creatureReceiveDamageEventHandler;

        public MonsterFactory(IMonsterDataManager monsterManager, CreatureInjuredEventHandler creatureReceiveDamageEventHandler)
        {
            _monsterManager = monsterManager;
            _creatureReceiveDamageEventHandler = creatureReceiveDamageEventHandler;
        }
        public IMonster Create(string name)
        {
            var result = _monsterManager.TryGetMonster(name, out IMonsterType monsterType);
            if (result == false)
            {
                throw new KeyNotFoundException($"Given monster name: {name} is not loaded");
            }
            
            var monster = new Monster(monsterType);

            monster.OnDamaged += _creatureReceiveDamageEventHandler.Execute;

            return monster;
        }
    }
}
