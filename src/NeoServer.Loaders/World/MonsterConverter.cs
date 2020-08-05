using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Enums.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Loaders.World
{
    public class MonsterConverter
    {
        public static IMonsterType Convert(MonsterData monsterData)
        {
            var data = monsterData.Monster;
            var monster = new MonsterType()
            {
                Name = data.Name,
                MaxHealth = data.Health.Max,
                Look = new Dictionary<LookType, ushort>() { { LookType.Type, data.Look.Type }, { LookType.Corpse, data.Look.Corpse } },
                Speed = data.Speed,
            };
            return monster;
        }
    }
}
