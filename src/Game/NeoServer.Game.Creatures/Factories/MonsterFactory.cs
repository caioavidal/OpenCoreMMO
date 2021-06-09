using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Creatures.Monsters;
using Serilog.Core;

namespace NeoServer.Game.Creatures
{
    public class MonsterFactory : IMonsterFactory
    {
        private readonly IMonsterDataManager _monsterManager;

        private readonly Logger logger;

        public MonsterFactory(IMonsterDataManager monsterManager,
            Logger logger)
        {
            _monsterManager = monsterManager;

            this.logger = logger;
            Instance = this;
        }

        public static IMonsterFactory Instance { get; private set; }

        public IMonster Create(string name, IMonster master)
        {
            var result = _monsterManager.TryGetMonster(name, out var monsterType);
            if (result == false)
            {
                logger.Warning($"Given monster name: {name} is not loaded");
                return null;
            }

            IMonster monster = new Summon(monsterType, master);

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

            return new Monster(monsterType, spawn);
        }
    }
}