using System;
using Newtonsoft.Json;

namespace NeoServer.Server.Helpers.JsonConverters;

public class AbstractConverter<TReal, TAbstract> : JsonConverter where TReal : TAbstract
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(TAbstract);
    }

    public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer jser)
    {
        return jser.Deserialize<TReal>(reader);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer jser)
    {
        jser.Serialize(writer, value);
    }
}