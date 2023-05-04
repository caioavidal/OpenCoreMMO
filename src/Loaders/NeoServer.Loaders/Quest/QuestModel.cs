using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NeoServer.Loaders.Quest;

public class QuestModel
{
    [JsonProperty("aid")] public ushort ActionId { get; set; }
    [JsonProperty("uid")] public uint UniqueId { get; set; }
    [JsonProperty("script")] public string Script { get; set; }
    [JsonProperty("rewards")] public List<Reward> Rewards { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("group")] public string Group { get; set; }
    [JsonProperty("group-key")] public string GroupKey { get; set; }

    [JsonProperty("auto-load")]
    [DefaultValue(true)]
    public bool AutoLoad { get; set; }

    public class Reward
    {
        [JsonProperty("id")] public ushort ItemId { get; set; }

        [JsonProperty("amount")] public byte Amount { get; set; }

        [JsonProperty("items")] public List<Reward> Children { get; set; }
    }
}