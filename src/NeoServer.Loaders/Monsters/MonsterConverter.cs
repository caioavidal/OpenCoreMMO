using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json.Linq;
using System.Linq;
using NeoServer.Game.Contracts.Combat.Defenses;
using NeoServer.Enums.Creatures.Enums;
using NeoServer.Loaders.Monsters.Converters;
using System.Collections.Immutable;

namespace NeoServer.Loaders.Monsters
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
                Armor = ushort.Parse(data.Defense.Armor),
                Defence = ushort.Parse(data.Defense.Defense),
                Defenses = new List<ICombatDefense>(),
                Experience = data.Experience
            };

            monster.TargetChance = new IntervalChance(System.Convert.ToUInt16(data.Targetchange.Interval), System.Convert.ToByte(data.Targetchange.Chance));

            monster.VoiceConfig = new IntervalChance(System.Convert.ToUInt16(data.Voices.Interval), System.Convert.ToByte(data.Voices.Chance));

            monster.Voices = data.Voices.Voice.Select(x=>x.Sentence).ToArray();

            monster.Attacks = MonsterAttackConverter.Convert(data);

            monster.Immunities = MonsterImmunityConverter.Convert(data).ToImmutableDictionary();


            //foreach (var defense in data.Defenses)
            //{
            //    defense.TryGetValue("name", out string defenseName);
            //    defense.TryGetValue("chance", out byte chance);
            //    defense.TryGetValue("interval", out ushort interval);
            //    defense.TryGetValue<JArray>("attributes", out var attributes);


            //    if (defenseName == "healing")
            //    {
            //        defense.TryGetValue("min", out decimal min);
            //        defense.TryGetValue("max", out decimal max);

            //        monster.Defenses.Add(new HealCombatDefence
            //        {
            //            Chance = chance,
            //            Interval = interval,
            //            Min = (ushort)Math.Abs(min),
            //            Max = (ushort)Math.Abs(max),
            //            Effect = ParseAreaEffect(attributes?.FirstOrDefault(a => a?.Value<string>("key") == "areaEffect")?.Value<string>("value"))

            //        });
            //    }
            //    if (defenseName == "speed")
            //    {
            //        defense.TryGetValue("speedchange", out ushort speed);
            //        defense.TryGetValue("duration", out uint duration);

            //        monster.Defenses.Add(new HasteCombatDefence()
            //        {
            //            Chance = chance,
            //            Interval = interval,
            //            SpeedBoost = speed,
            //            Duration = duration,
            //            Effect = ParseAreaEffect(attributes?.FirstOrDefault(a => a.Value<string>("key") == "areaEffect")?.Value<string>("value"))
            //        });
            //    }
            //}

            foreach (var flag in data.Flags)
            {
                var creatureFlag = ParseCreatureFlag(flag.Key);
                monster.Flags.Add(creatureFlag, flag.Value);
            }

            return monster;
        }

        private static DamageType ParseDamageType(string type)
        {
            return type switch
            {
                "melee" => DamageType.Melee,
                "physical" => DamageType.Physical,
                "energy" => DamageType.Energy,
                "fire" => DamageType.Fire,
                "manadrain" => DamageType.ManaDrain,
                _ => DamageType.Melee
            };
        }
        private static EffectT ParseAreaEffect(string type)
        {
            return type switch
            {
                "blueshimmer" => EffectT.GlitterBlue,
                "redshimmer" => EffectT.GlitterRed,
                _ => EffectT.None
            };
        }
        private static ShootType ParseShootType(string type)
        {
            return type switch
            {
                "bolt" => ShootType.Bolt,
                "spear" => ShootType.Spear,
                "star" => ShootType.ThrowingStar,
                "energy" => ShootType.Energy,
                "fire" => ShootType.Fire,
                _ => ShootType.None
            };
        }
        private static CreatureFlagAttribute ParseCreatureFlag(string flag)
        {
            return flag switch
            {
                "summonable" => CreatureFlagAttribute.Summonable,
                "attackable" => CreatureFlagAttribute.Attackable,
                "hostile" => CreatureFlagAttribute.Hostile,
                "illusionable" => CreatureFlagAttribute.Illusionable,
                "convinceable" => CreatureFlagAttribute.Convinceable,
                "pushable" => CreatureFlagAttribute.Pushable,
                "canpushitems" => CreatureFlagAttribute.CanPushItems,
                "canpushcreatures" => CreatureFlagAttribute.CanPushCreatures,
                "targetdistance" => CreatureFlagAttribute.TargetDistance,
                "staticattack" => CreatureFlagAttribute.StaticAttack,
                "runonhealth" => CreatureFlagAttribute.RunOnHealth,
                _ => CreatureFlagAttribute.None
            };
        }
    }
}
