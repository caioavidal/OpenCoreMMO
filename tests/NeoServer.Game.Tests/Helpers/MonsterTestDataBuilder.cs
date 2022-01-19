using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Monsters;
using NeoServer.Game.World.Services;
using NeoServer.Game.World.Spawns;
using PathFinder = NeoServer.Game.World.Map.PathFinder;

namespace NeoServer.Game.Tests.Helpers
{
    public static class MonsterTestDataBuilder
    {
        public static IMonster Build(uint maxHealth = 100)
        {
            var map = MapTestDataBuilder.Build(100, 110, 100, 110, 7, 7, addGround: true);
            var pathFinder = new PathFinder(map);
            var spawnPoint = new SpawnPoint(new Location(105, 105, 7), 60);

            var mapTool = new MapTool(map, pathFinder);

            var monsterType = new MonsterType()
            {
                Name = "Monster X",
                MaxHealth = maxHealth,
                Attacks = new IMonsterCombatAttack[]
                {
                    new MonsterCombatAttack()
                    {
                        MinDamage = 10,
                        MaxDamage = 100,
                        Interval = 1000,
                        DamageType = DamageType.Melee,
                        Chance = 100,
                        CombatAttack = new MeleeCombatAttack()
                        {
                            Min = 10,
                            Max = 100
                        }
                    }
                }
            };
            return new Monster(monsterType, mapTool,spawnPoint );
        }
        
        public static IMonster BuildSummon(ICreature master, ushort minDamage= 10, ushort maxDamage = 100)
        {
            var map = MapTestDataBuilder.Build(100, 110, 100, 110, 7, 7, addGround: true);
            var pathFinder = new PathFinder(map);
            var spawnPoint = new SpawnPoint(new Location(105, 105, 7), 60);

            var mapTool = new MapTool(map, pathFinder);

            var monsterType = new MonsterType()
            {
                Name = "Monster X",
                MaxHealth = 100,
                Attacks = new IMonsterCombatAttack[]
                {
                    new MonsterCombatAttack()
                    {
                        MinDamage = minDamage,
                        MaxDamage = maxDamage,
                        Interval = 1,
                        DamageType = DamageType.Melee,
                        Chance = 100,
                        CombatAttack = new MeleeCombatAttack()
                        {
                            Min = minDamage,
                            Max = maxDamage
                        }
                    }
                }
            };
            return new Summon(monsterType, mapTool,master );
        }
    }
}