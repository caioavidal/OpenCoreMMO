using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creatures.Model.Monsters;
using Serilog.Core;

namespace NeoServer.Game.Creatures
{
    public class MonsterFactory : IMonsterFactory
    {
        private readonly IMonsterDataManager _monsterManager;
     
        private readonly IPathAccess pathAccess;
        private readonly Logger logger;       

        public static IMonsterFactory Instance { get; private set; }

        public MonsterFactory(IMonsterDataManager monsterManager,
            CreaturePathAccess creaturePathAccess, 
            Logger logger)
        {
            _monsterManager = monsterManager;
            pathAccess = creaturePathAccess;
            this.logger = logger;
            Instance = this;

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
            return monster;
        }

    }
}