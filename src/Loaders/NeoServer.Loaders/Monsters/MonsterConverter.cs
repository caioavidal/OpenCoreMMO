using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Loaders.Monsters.Converters;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NeoServer.Loaders.Monsters
{
    public class MonsterConverter
    {
        public static IMonsterType Convert(MonsterData monsterData, GameConfiguration configuration, IMonsterDataManager monsters)
        {
            var data = monsterData;
            var monster = new MonsterType()
            {
                Name = data.Name,
                MaxHealth = data.Health.Max,
                Look = new Dictionary<LookType, ushort>() { { LookType.Type, data.Look.Type }, { LookType.Corpse, data.Look.Corpse }, { LookType.Body, data.Look.Body}, { LookType.Legs, data.Look.Legs}, { LookType.Head, data.Look.Head },
                { LookType.Feet, data.Look.Feet},{ LookType.Addon, data.Look.Addons}},
                Speed = data.Speed,
                Armor = ushort.Parse(data.Defense.Armor),
                Defense = ushort.Parse(data.Defense.Defense),
                Experience = (uint)(data.Experience * configuration.ExperienceRate),
                Race = ParseRace(data.Race)
            };

            monster.TargetChance = new IntervalChance(System.Convert.ToUInt16(data.Targetchange.Interval), System.Convert.ToByte(data.Targetchange.Chance));

            if (data.Voices != null)
            {
                monster.VoiceConfig = new IntervalChance(System.Convert.ToUInt16(data.Voices.Interval), System.Convert.ToByte(data.Voices.Chance));
                monster.Voices = data.Voices.Sentences.Select(x => x.Sentence).ToArray();
            }

            monster.Attacks = MonsterAttackConverter.Convert(data);

            monster.Immunities = MonsterImmunityConverter.Convert(data).ToImmutableDictionary();

            monster.Defenses = MonsterDefenseConverter.Convert(data, monsters);

            monster.Loot = MonsterLootConverter.Convert(data, configuration.LootRate);

            foreach (var flag in data.Flags)
            {
                var creatureFlag = ParseCreatureFlag(flag.Key);
                monster.Flags.Add(creatureFlag, flag.Value);
            }

            return monster;
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
                "lightcolor" => CreatureFlagAttribute.LightColor,
                _ => CreatureFlagAttribute.None
            };
        }

        private static Race ParseRace(string race) => race switch
        {
            "venom" => Race.Venom,
            "blood" => Race.Bood,
            "undead" => Race.Undead,
            "fire" => Race.Fire,
            "energy" => Race.Energy,
            _ => Race.Bood
        };
    }
}
