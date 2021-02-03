using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Loaders.Npcs
{
    public class NpcData
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("walk-interval")]
        public int WalkInterval { get; set; }
        [JsonProperty("health")]
        public HealthData Health { get; set; }
        [JsonProperty("look")]
        public LookData Look { get; set; }
        public string[] Marketings { get; set; }
        public DialogData[] Dialog { get; set; }
        public string Script { get; set; }

        public class DialogData
        {
            [JsonProperty("words")]
            public string[] OnWords { get; set; }
            public string[] Answers { get; set; }
            public DialogData[] Then { get; set; }
            public string Action { get; set; }
            public bool End { get; set; }
        }
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
    }
}
