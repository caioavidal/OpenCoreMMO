using Microsoft.VisualBasic;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
                Armor = ushort.Parse(data.Defenses.Armor),
                Defence = ushort.Parse(data.Defenses.Defense),
                Attacks = new Dictionary<DamageType, ICombatAttack>(),
                Experience = data.Experience
            };

            if (data.Attacks is Collection attacks)
            {

            }
            else
            {
                var a = (JObject)data.Attacks;
                var name = a["attack"]["name"].ToString();

                monster.Attacks.Add(ParseDamageType(name), new CombatAttack()
                {
                    Name = name,
                    Interval = ushort.Parse(a["attack"]["interval"].ToString()),
                    Skill = ushort.Parse(a["attack"]["skill"].ToString()),
                    Attack = ushort.Parse(a["attack"]["attack"].ToString()),
                });
            }

            return monster;
        }

        private static DamageType ParseDamageType(string type)
        {
            return type switch
            {
                "melee" => DamageType.Melee,
                _ => DamageType.Melee
            };
        }
    }
}
