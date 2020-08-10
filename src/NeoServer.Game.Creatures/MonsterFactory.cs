using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
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
        private readonly CreatureWasBornEventHandler _creatureWasBornEventHandler;
        private readonly CreatureBlockedAttackEventHandler _creatureBlockedAttackEventHandler;

        public MonsterFactory(IMonsterDataManager monsterManager, CreatureInjuredEventHandler creatureReceiveDamageEventHandler, CreatureKilledEventHandler creatureKilledEventHandler, 
            CreatureWasBornEventHandler creatureWasBornEventHandler, CreatureBlockedAttackEventHandler creatureBlockedAttackEventHandler)
        {
            _monsterManager = monsterManager;
            _creatureReceiveDamageEventHandler = creatureReceiveDamageEventHandler;
            _creatureKilledEventHandler = creatureKilledEventHandler;
            _creatureWasBornEventHandler = creatureWasBornEventHandler;
            _creatureBlockedAttackEventHandler = creatureBlockedAttackEventHandler;
        }
        public IMonster Create(string name, ISpawnPoint spawn = null)
        {
            var result = _monsterManager.TryGetMonster(name, out IMonsterType monsterType);
            if (result == false)
            {
                throw new KeyNotFoundException($"Given monster name: {name} is not loaded");
            }
            
            var monster = new Monster(monsterType, spawn);

            monster.OnDamaged += _creatureReceiveDamageEventHandler.Execute;
            monster.OnKilled += _creatureKilledEventHandler.Execute;
            monster.OnWasBorn += _creatureWasBornEventHandler.Execute;
            monster.OnBlockedAttack += _creatureBlockedAttackEventHandler.Execute;
            return monster;
        }
    }
}
