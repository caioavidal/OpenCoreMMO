using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NeoServer.Loaders.Chats;

[Serializable]
public class ChannelModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Enabled { get; set; } = true;
    public bool Opened { get; set; }

    [JsonProperty("allowed-vocations")] public byte[] Vocations { get; set; }

    [JsonProperty("allowed-levels")] public LevelModel Level { get; set; }

    [JsonProperty("write-rules")] public WriteModel Write { get; set; }

    public ColorModel Color { get; set; }

    [JsonProperty("mute-rule")] public MuteRuleModel MuteRule { get; set; }

    public string Script { get; set; }

    public class LevelModel
    {
        [JsonProperty("bigger-than")] public ushort BiggerThan { get; set; }

        [JsonProperty("lower-than")] public ushort LowerThan { get; set; }
    }

    public class WriteModel
    {
        [JsonProperty("allowed-vocations")] public byte[] Vocations { get; set; }

        [JsonProperty("allowed-levels")] public LevelModel Level { get; set; }
    }

    [Serializable]
    public class ColorModel
    {
        public string Default { get; set; }

        [JsonProperty("by-vocation")] public Dictionary<int, string> ByVocation { get; set; }
    }

    public class MuteRuleModel
    {
        [JsonProperty("messages-count")] public ushort MessagesCount { get; set; }

        [JsonProperty("time-to-block")] public ushort TimeToBlock { get; set; }

        [JsonProperty("wait-time")] public ushort WaitTime { get; set; }

        [JsonProperty("time-multiplier")] public double TimeMultiplier { get; set; }

        [JsonProperty("cancel-message")] public string CancelMessage { get; set; }
    }
}