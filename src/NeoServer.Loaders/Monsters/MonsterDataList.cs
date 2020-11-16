﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace NeoServer.Loaders.Monsters
{
    public class MonsterData
    {
        [JsonProperty("monster")]
        public MonsterMetadata Monster { get; set; }

        public class Health
        {
            [JsonProperty("now")]
            public ushort Now { get; set; }

            [JsonProperty("max")]
            public ushort Max { get; set; }
        }

        public class Look
        {
            [JsonProperty("type")]
            public ushort Type { get; set; }

            [JsonProperty("corpse")]
            public ushort Corpse { get; set; }
        }

        public class Targetchange
        {
            [JsonProperty("interval")]
            public string Interval { get; set; }

            [JsonProperty("chance")]
            public string Chance { get; set; }
        }

        public class Strategy
        {
            [JsonProperty("attack")]
            public string Attack { get; set; }

            [JsonProperty("defense")]
            public string Defense { get; set; }
        }

        public class Flags
        {
            [JsonProperty("flag")]
            public List<IDictionary<string, byte>> Flag { get; set; }
        }

        public class CombatAttack
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("interval")]
            public string Interval { get; set; }

            [JsonProperty("skill")]
            public string Skill { get; set; }

            [JsonProperty("attack")]
            public string Attack { get; set; }
        }

        public class Attacks
        {
            [JsonProperty("attack")]
            public CombatAttack Attack { get; set; }
        }

        public class Defenses
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
        }

        public class Voices
        {
            [JsonProperty("interval")]
            public string Interval { get; set; }

            [JsonProperty("chance")]
            public string Chance { get; set; }

            [JsonProperty("voice")]
            public List<Voice> Voice { get; set; }
        }

        public class Item2
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("chance")]
            public string Chance { get; set; }
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
            public List<Item2> Item { get; set; }
        }

        public class Loot
        {
            [JsonProperty("item")]
            public List<ItemData> Item { get; set; }

            [JsonProperty("#comment")]
            public List<object> Comment { get; set; }
        }

        public class MonsterMetadata
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
            public Health Health { get; set; }

            [JsonProperty("look")]
            public Look Look { get; set; }

            [JsonProperty("targetchange")]
            public Targetchange Targetchange { get; set; }

            [JsonProperty("strategy")]
            public Strategy Strategy { get; set; }

            [JsonProperty("flags")]
            public IDictionary<string, byte> Flags { get; set; }

            [JsonProperty("attacks")]
            public List<Dictionary<string, object>> Attacks { get; set; } = new List<Dictionary<string, object>>();

            [JsonProperty("defenses")]
            public List<Dictionary<string, object>> Defenses { get; set; } = new List<Dictionary<string, object>>();
            [JsonProperty("defense")]
            public Defenses Defense { get; set; }

            [JsonProperty("elements")]
            public Dictionary<string,sbyte> Elements { get; set; }

            [JsonProperty("voices")]
            public Voices Voices { get; set; }

            [JsonProperty("loot")]
            public Loot Loot { get; set; }
        }
    }
}