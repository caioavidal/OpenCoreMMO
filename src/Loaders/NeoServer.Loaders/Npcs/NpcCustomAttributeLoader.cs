using System.Collections.Generic;
using System.Dynamic;
using NeoServer.Game.Common.Contracts.Creatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NeoServer.Loaders.Npcs;

public class NpcCustomAttributeLoader
{
    public static void LoadCustomData(INpcType type, NpcJsonData npcData)
    {
        if (type is null || npcData?.CustomData is null) return;

        var jsonString = JsonConvert.SerializeObject(npcData.CustomData);

        var converter = new ExpandoObjectConverter();

        var list = JsonConvert.DeserializeObject<ExpandoObject[]>(jsonString, converter);

        var map = new Dictionary<string, dynamic>();

        foreach (var item in list) map.TryAdd(item.key, item.value);

        type.CustomAttributes.Add("custom-data", map);
    }
}