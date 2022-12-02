using Newtonsoft.Json;

namespace NeoServer.Loaders.Quest;

public class QuestModel
{
    [JsonProperty("aid")] public ushort ActionId { get; set; }
    [JsonProperty("uid")] public uint UniqueId { get; set; }
    [JsonProperty("script")] public string Script { get; set; }
  //  [JsonProperty("rewards")] public List<> Script { get; set; }
}