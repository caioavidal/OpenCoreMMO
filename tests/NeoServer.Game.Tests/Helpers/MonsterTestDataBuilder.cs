using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Monsters;
using NeoServer.Game.World.Spawns;
using PathFinder = NeoServer.Game.World.Map.PathFinder;

namespace NeoServer.Game.Tests.Helpers
{
    public static class MonsterTestDataBuilder
    {
        public static IMonster Build()
        {
            var map = MapTestDataBuilder.Build(100, 110, 100, 110, 7, 7, addGround: true);
            var pathFinder = new PathFinder(map);
            var spawnPoint = new SpawnPoint(new Location(105, 105, 7), 60);

            var monsterType = new MonsterType()
            {
                Name = "Monster X"
            };
            return new Monster(monsterType, pathFinder,spawnPoint );
        }
    }
}