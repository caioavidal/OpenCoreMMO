using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Creatures.Monsters;
using Serilog;
using Serilog.Core;

namespace NeoServer.Game.Creatures.Factories
{
    public class MonsterFactory : IMonsterFactory
    {
        private readonly IMonsterDataManager _monsterManager;
        private readonly IPathFinder _pathFinder;

        private readonly ILogger logger;

        public MonsterFactory(IMonsterDataManager monsterManager, IPathFinder pathFinder,
            ILogger logger)
        {
            _monsterManager = monsterManager;
            _pathFinder = pathFinder;

            this.logger = logger;
            Instance = this;
        }

        public static IMonsterFactory Instance { get; private set; }

        public IMonster CreateSummon(string name, IMonster master)
        {
            var result = _monsterManager.TryGetMonster(name, out var monsterType);
            if (result == false)
            {
                logger.Warning($"Given monster name: {name} is not loaded");
                return null;
            }

            IMonster monster = new Summon(monsterType,_pathFinder, master);

            return monster;
        }

        public IMonster Create(string name, ISpawnPoint spawn = null)
        {
            var result = _monsterManager.TryGetMonster(name, out var monsterType);
            if (result == false)
            {
                logger.Warning($"Given monster name: {name} is not loaded");
                return null;
            }

            return new Monster(monsterType,_pathFinder, spawn);
        }
    }
}