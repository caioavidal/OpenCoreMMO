using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Server.Events.Combat;
using NeoServer.Server.Events.Creature;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures
{
    public class MonsterFactory : IMonsterFactory
    {
        private readonly IMonsterDataManager _monsterManager;
        private readonly CreatureWasBornEventHandler _creatureWasBornEventHandler;
        private readonly CreatureAttackEventHandler _creatureAttackEventHandler;
        private readonly MonsterDefendEventHandler _monsterDefendEventHandler;
        private readonly IPathFinder _pathFinder;

        public MonsterFactory(IMonsterDataManager monsterManager,
            CreatureWasBornEventHandler creatureWasBornEventHandler, IPathFinder pathFinder, CreatureAttackEventHandler creatureAttackEventHandler, MonsterDefendEventHandler monsterDefendEventHandler)
        {
            _monsterManager = monsterManager;
            _creatureWasBornEventHandler = creatureWasBornEventHandler;
            _pathFinder = pathFinder;
            _creatureAttackEventHandler = creatureAttackEventHandler;
            _monsterDefendEventHandler = monsterDefendEventHandler;
        }
        public IMonster Create(string name, ISpawnPoint spawn = null)
        {
            var result = _monsterManager.TryGetMonster(name, out IMonsterType monsterType);
            if (result == false)
            {
                throw new KeyNotFoundException($"Given monster name: {name} is not loaded");
            }
            var monster = new Monster(monsterType, _pathFinder.Find, spawn);

            monster.OnWasBorn += _creatureWasBornEventHandler.Execute;
            monster.OnAttackEnemy += _creatureAttackEventHandler.Execute;
            monster.OnDefende += _monsterDefendEventHandler.Execute;
            monster.OnKilled += DetachEvents;

            return monster;
        }
        public void DetachEvents(ICreature creature)
        {
            if (creature is IMonster monster)
            {
                monster.OnWasBorn -= _creatureWasBornEventHandler.Execute;
                monster.OnAttackEnemy -= _creatureAttackEventHandler.Execute;
                monster.OnDefende -= _monsterDefendEventHandler.Execute;
                monster.OnKilled -= DetachEvents;
            }
        }
    }
}
