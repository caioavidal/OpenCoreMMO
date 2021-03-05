using NeoServer.Game.Contracts.Creatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Loaders.Npcs
{
    public class NpcCustomAttributeLoader
    {
        public static void LoadCustomData(INpcType type, NpcData npcData)
        {
            if (type is null || npcData is null || npcData.CustomData is null) return;

            var jsonString = JsonConvert.SerializeObject(npcData.CustomData);

            var converter = new ExpandoObjectConverter();

        
            if (npcData.CustomData is JArray)
            {
                type.CustomAttributes.Add("custom-data", JsonConvert.DeserializeObject<ExpandoObject[]>(jsonString, converter));
            }
            else if (npcData.CustomData is JObject)
            {
                type.CustomAttributes.Add("custom-data", JsonConvert.DeserializeObject<ExpandoObject>(jsonString, converter));
            }
            else
            {
                type.CustomAttributes.Add("custom-data", npcData.CustomData);
            }
        }
    }
}
