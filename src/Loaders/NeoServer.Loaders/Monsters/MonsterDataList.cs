using Newtonsoft.Json;
using System.Collections.Generic;

namespace NeoServer.Loaders.Monsters
{
    public class MonsterData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nameDescription")]
        public string NameDescription { get; set; }

        [JsonProperty("race")]
        public string Race { get; set; }

        [JsonProperty("experience")]
        public uint Experience { get; set; }

        [JsonProperty("speed")]
        public ushort Speed { get; set; }

        [JsonProperty("manacost")]
        public ushort Manacost { get; set; }

        [JsonProperty("health")]
        public HealthData Health { get; set; }

        [JsonProperty("look")]
        public LookData Look { get; set; }

        [JsonProperty("targetchange")]
        public TargetchangeData Targetchange { get; set; }

        [JsonProperty("strategy")]
        public StrategyData Strategy { get; set; }

        [JsonProperty("flags")]
        public IDictionary<string, ushort> Flags { get; set; }

        [JsonProperty("attacks")]
        public List<Dictionary<string, object>> Attacks { get; set; } = new List<Dictionary<string, object>>();

        [JsonProperty("defenses")]
        public List<Dictionary<string, object>> Defenses { get; set; } = new List<Dictionary<string, object>>();
        [JsonProperty("defense")]
        public DefenseData Defense { get; set; }

        [JsonProperty("elements")]
        public Dictionary<string, sbyte> Elements { get; set; }
        [JsonProperty("voices")]
        public VoicesData Voices { get; set; }

        [JsonProperty("loot")]
        public List<LootData> Loot { get; set; }

        public class HealthData
        {
            [JsonProperty("now")]
            public uint Now { get; set; }

            [JsonProperty("max")]
            public uint Max { get; set; }
        }

        public class LookData
        {
            [JsonProperty("type")]
            public ushort Type { get; set; }

            [JsonProperty("corpse")]
            public ushort Corpse { get; set; }
            [JsonProperty("body")]
            public ushort Body { get; set; }
            [JsonProperty("legs")]
            public ushort Legs { get; set; }
            [JsonProperty("feet")]
            public ushort Feet { get; set; }
            [JsonProperty("head")]
            public ushort Head { get; set; }
            [JsonProperty("addons")]
            public ushort Addons { get; set; }
        }

        public class TargetchangeData
        {
            [JsonProperty("interval")]
            public string Interval { get; set; }

            [JsonProperty("chance")]
            public string Chance { get; set; }
        }

        public class StrategyData
        {
            [JsonProperty("attack")]
            public string Attack { get; set; }

            [JsonProperty("defense")]
            public string Defense { get; set; }
        }      
        public class DefenseData
        {
            [JsonProperty("armor")]
            public string Armor { get; set; }

            [JsonProperty("defense")]
            public string Defense { get; set; }
        }

        public class Voice
        {
            [JsonProperty("sentence")]
            public string Sentence { get; set; }
            [JsonProperty("yell")]
            public bool Yell { get; set; }
        }

        public class VoicesData
        {
            [JsonProperty("interval")]
            public string Interval { get; set; }

            [JsonProperty("chance")]
            public string Chance { get; set; }

            [JsonProperty("sentences")]
            public List<Voice> Sentences { get; set; }
        }

        public class ItemData
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("countmax")]
            public string Countmax { get; set; }

            [JsonProperty("chance")]
            public string Chance { get; set; }

            [JsonProperty("item")]
            public List<ItemData> Item { get; set; }
        }

        public class LootData
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("countmax")]
            public string Countmax { get; set; }

            [JsonProperty("chance")]
            public string Chance { get; set; }

            [JsonProperty("items")]
            public List<LootData> Items { get; set; }
        }

    }
}
