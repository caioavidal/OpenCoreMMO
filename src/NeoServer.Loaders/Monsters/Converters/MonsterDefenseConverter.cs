using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Combat.Defenses;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoServer.Loaders.Monsters.Converters
{
    public class MonsterDefenseConverter
    {
        public static ICombatDefense[] Convert(MonsterData data)
        {
            if (data.Defenses is null) return new ICombatDefense[0];

            var defenses = new List<ICombatDefense>();

            foreach (var defense in data.Defenses)
            {
                defense.TryGetValue("name", out string defenseName);
                defense.TryGetValue("chance", out byte chance);
                defense.TryGetValue("interval", out ushort interval);
                defense.TryGetValue<JArray>("attributes", out var attributes);


                if (defenseName == "healing")
                {
                    defense.TryGetValue("min", out decimal min);
                    defense.TryGetValue("max", out decimal max);

                    defenses.Add(new HealCombatDefense
                    {
                        Chance = chance,
                        Interval = interval,
                        Min = (int)Math.Abs(min),
                        Max = (int)Math.Abs(max),
                        Effect = MonsterAttributeParser.ParseAreaEffect(attributes?.FirstOrDefault(a => a?.Value<string>("key") == "areaEffect")?.Value<string>("value"))
                    });
                }
                if (defenseName == "speed")
                {
                    defense.TryGetValue("speedchange", out ushort speed);
                    defense.TryGetValue("duration", out uint duration);

                    defenses.Add(new HasteCombatDefense()
                    {
                        Chance = chance,
                        Interval = interval,
                        SpeedBoost = speed,
                        Duration = duration,
                        Effect = MonsterAttributeParser.ParseAreaEffect(attributes?.FirstOrDefault(a => a.Value<string>("key") == "areaEffect")?.Value<string>("value"))
                    });
                }
            }

            return defenses.ToArray();
        }
    }
}