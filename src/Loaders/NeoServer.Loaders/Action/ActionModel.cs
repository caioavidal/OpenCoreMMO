using Newtonsoft.Json;

namespace NeoServer.Loaders.Action;

public class ActionModel
{
    [JsonProperty("fromId")]
    public ushort FromId { get; set; }
    
    [JsonProperty("script")]
    public string Script { get; set; }
}