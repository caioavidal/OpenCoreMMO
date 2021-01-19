using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Combat.Defenses;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Loaders.Monsters.Converters
{
    public class MonsterDefenseConverter
    {
        public static ICombatDefense[] Convert(MonsterData data, IMonsterDataManager monsters)
        {
            if (data.Defenses is null) return Array.Empty<ICombatDefense>();

            var defenses = new List<ICombatDefense>();

            foreach (var defense in data.Defenses)
            {
                defense.TryGetValue("name", out string defenseName);
                defense.TryGetValue("chance", out byte chance);
                defense.TryGetValue("interval", out ushort interval);

                defense.TryGetValue<JArray>("attributes", out var attributesArray);
                var attributes = attributesArray?.ToDictionary(k => ((JObject)k).Properties().First().Name, v => v.Values().First().Value<object>());

                attributes.TryGetValue("areaEffect", out string areaEffect);

                if (defenseName.Equals("healing", StringComparison.InvariantCultureIgnoreCase))
                {
                    defense.TryGetValue("min", out decimal min);
                    defense.TryGetValue("max", out decimal max);

                    defenses.Add(new HealCombatDefense((int)Math.Abs(min), (int)Math.Abs(max), MonsterAttributeParser.ParseAreaEffect(areaEffect))
                    {
                        Chance = chance,
                        Interval = interval
                    });
                }
                else if (defenseName.Equals("speed", StringComparison.InvariantCultureIgnoreCase))
                {
                    defense.TryGetValue("speedchange", out ushort speed);
                    defense.TryGetValue("duration", out uint duration);

                    defenses.Add(new HasteCombatDefense(duration, speed, MonsterAttributeParser.ParseAreaEffect(areaEffect))
                    {
                        Chance = chance,
                        Interval = interval
                    });
                }
                else if (defenseName.Equals("invisible", StringComparison.InvariantCultureIgnoreCase))
                {
                    defense.TryGetValue("duration", out uint duration);

                    defenses.Add(new InvisibleCombatDefense(duration, MonsterAttributeParser.ParseAreaEffect(areaEffect))
                    {
                        Chance = chance,
                        Interval = interval,
                    });
                }
                else if (defenseName.Equals("outfit", StringComparison.InvariantCultureIgnoreCase))
                {
                    defense.TryGetValue("duration", out uint duration);
                    defense.TryGetValue("monster", out string monsterName);

                    defenses.Add(new IllusionCombatDefense(duration, monsterName, MonsterAttributeParser.ParseAreaEffect(areaEffect), monsters)
                    {
                        Chance = chance,
                        Interval = interval,
                    });
                }
                else
                {
                    Console.WriteLine($"{defenseName} defense was not created on monster: {data.Name}");
                }
            }

            return defenses.ToArray();
        }
    }
}