using System;
using Newtonsoft.Json;

namespace NeoServer.Loaders.Npcs;

[Serializable]
public class NpcJsonData
{
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("walk-interval")] public int WalkInterval { get; set; }

    [JsonProperty("health")] public HealthData Health { get; set; }

    [JsonProperty("look")] public LookData Look { get; set; }

    public string[] Marketings { get; set; }
    public DialogData[] Dialog { get; set; }
    public string Script { get; set; }
    public ShopData[] Shop { get; set; }

    [JsonProperty("custom-data")] public dynamic CustomData { get; set; }

    [Serializable]
    public class DialogData
    {
        [JsonProperty("words")] public string[] Words { get; set; }

        public string[] Answers { get; set; }
        public DialogData[] Then { get; set; }
        public string Action { get; set; }
        public bool End { get; set; }

        [JsonProperty("store-at")] public string StoreAt { get; set; }

        public byte Back { get; set; }
    }

    public class HealthData
    {
        [JsonProperty("now")] public uint Now { get; set; }

        [JsonProperty("max")] public uint Max { get; set; }
    }

    public class LookData
    {
        [JsonProperty("type")] public ushort Type { get; set; }

        [JsonProperty("corpse")] public ushort Corpse { get; set; }

        [JsonProperty("body")] public ushort Body { get; set; }

        [JsonProperty("legs")] public ushort Legs { get; set; }

        [JsonProperty("feet")] public ushort Feet { get; set; }

        [JsonProperty("head")] public ushort Head { get; set; }

        [JsonProperty("addons")] public ushort Addons { get; set; }
    }

    [Serializable]
    public class ShopData
    {
        public ushort Item { get; set; }
        public uint Sell { get; set; }
        public uint Buy { get; set; }
    }
}