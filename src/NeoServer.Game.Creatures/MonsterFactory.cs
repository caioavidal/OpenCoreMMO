using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Events;
using NeoServer.Server.Events.Creature;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures
{
    public class MonsterFactory : IMonsterFactory
    {
        private readonly IMonsterDataManager _monsterManager;
        private readonly CreatureInjuredEventHandler _creatureReceiveDamageEventHandler;
        private readonly CreatureKilledEventHandler _creatureKilledEventHandler;

        public MonsterFactory(IMonsterDataManager monsterManager, CreatureInjuredEventHandler creatureReceiveDamageEventHandler, CreatureKilledEventHandler creatureKilledEventHandler)
        {
            _monsterManager = monsterManager;
            _creatureReceiveDamageEventHandler = creatureReceiveDamageEventHandler;
            _creatureKilledEventHandler = creatureKilledEventHandler;
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
            monster.OnKilled += _creatureKilledEventHandler.Execute;

            return monster;
        }
    }
}
