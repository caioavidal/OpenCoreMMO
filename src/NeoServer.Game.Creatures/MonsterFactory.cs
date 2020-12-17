using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Server.Events.Combat;
using NeoServer.Server.Events.Creature;
using NeoServer.Server.Events.Creature.Monsters;
using Serilog.Core;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures
{
    public class MonsterFactory : IMonsterFactory
    {
        private readonly IMonsterDataManager _monsterManager;
        private readonly CreatureWasBornEventHandler _creatureWasBornEventHandler;
        private readonly CreatureAttackEventHandler _creatureAttackEventHandler;
        private readonly CreatureDroppedLootEventHandler creatureDroppedLootEventHandler;
        private readonly MonsterChangedStateEventHandler monsterChangedStateEventHandler;
        private readonly IPathAccess pathAccess;
        private readonly Logger logger;

        public MonsterFactory(IMonsterDataManager monsterManager,
            CreatureWasBornEventHandler creatureWasBornEventHandler, CreatureAttackEventHandler creatureAttackEventHandler,
            IItemFactory itemFactory, CreatureDroppedLootEventHandler creatureDroppedLootEventHandler, CreaturePathAccess creaturePathAccess, Logger logger,
            MonsterChangedStateEventHandler monsterChangedStateEventHandler)
        {
            _monsterManager = monsterManager;
            _creatureWasBornEventHandler = creatureWasBornEventHandler;
            _creatureAttackEventHandler = creatureAttackEventHandler;
            this.creatureDroppedLootEventHandler = creatureDroppedLootEventHandler;
            pathAccess = creaturePathAccess;
            this.logger = logger;
            this.monsterChangedStateEventHandler = monsterChangedStateEventHandler;
        }
        public IMonster Create(string name, ISpawnPoint spawn = null)
        {
            var result = _monsterManager.TryGetMonster(name, out IMonsterType monsterType);
            if (result == false)
            {
                logger.Warning($"Given monster name: {name} is not loaded");
                return null;
            }
            var monster = new Monster(monsterType, pathAccess, spawn);

            monster.OnWasBorn += _creatureWasBornEventHandler.Execute;
            monster.OnAttackEnemy += _creatureAttackEventHandler.Execute;
            monster.OnDropLoot += creatureDroppedLootEventHandler.Execute;
            monster.OnChangedState += monsterChangedStateEventHandler.Execute;
            return monster;
        }

    }
}