using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeoServer.Loaders.Spawns;

[Serializable]
public class SpawnData
{
    public ushort CenterX { get; set; }
    public ushort CenterY { get; set; }
    public byte CenterZ { get; set; }
    
    public byte Radius { get; set; }
    
    [JsonProperty("Monsters")]
    [JsonConverter(typeof(ArrayOrObjectConverter<Creature>))]
    public IEnumerable<Creature> Monsters { get; set; }
    
    [JsonProperty("Monster")]
    [JsonConverter(typeof(ArrayOrObjectConverter<Creature>))]
    private IEnumerable<Creature> Monster { set => Monsters = value; }
    
    [JsonProperty("Npcs")]
    [JsonConverter(typeof(ArrayOrObjectConverter<Creature>))]
    public IEnumerable<Creature> Npcs { get; set; }
    
    [JsonProperty("Npc")]
    [JsonConverter(typeof(ArrayOrObjectConverter<Creature>))]
    private IEnumerable<Creature> Npc { set => Npcs = value; }

    [Serializable]
    public class Creature
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public byte Z { get; set; }
        public ushort SpawnTime { get; set; }
    }
}

class ArrayOrObjectConverter<T> : JsonConverter
{
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        return token.Type == JTokenType.Array
            ? token.ToObject<List<T>>()
            : new List<T> { token.ToObject<T>() };
    }

    public override bool CanConvert(Type objectType)
        => objectType == typeof(List<T>);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        =>throw new NotImplementedException(); 
}