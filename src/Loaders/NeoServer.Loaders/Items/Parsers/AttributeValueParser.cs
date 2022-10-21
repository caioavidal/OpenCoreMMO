using System;
using Newtonsoft.Json.Linq;

namespace NeoServer.Loaders.Items.Parsers;

public static class AttributeValueParser
{
    public static dynamic Parse(object value)
    {
        if (value is null) return null;

        if (value is JArray array) return ParseArray(array);

        return value;
    }

    private static dynamic ParseArray(JArray jArray)
    {
        return jArray switch
        {
            { First.Type: JTokenType.Boolean } => jArray.ToObject<bool[]>(),
            { First.Type: JTokenType.Date } => jArray.ToObject<DateTime[]>(),
            { First.Type: JTokenType.Integer } => jArray.ToObject<long[]>(),
            { First.Type: JTokenType.Float } => jArray.ToObject<float[]>(),
            { First.Type: JTokenType.Guid } => jArray.ToObject<string[]>(),
            { First.Type: JTokenType.String } => jArray.ToObject<string[]>(),
            _ => jArray.ToObject<object[]>()
        };
    }
}