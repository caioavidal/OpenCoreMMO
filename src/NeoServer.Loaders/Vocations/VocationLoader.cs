using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Vocations;
using NeoServer.Server.Helpers.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Loaders.Vocations
{
    public class VocationLoader
    {
        
        private readonly GameConfiguration gameConfiguration;
        private readonly Logger logger;
        public VocationLoader(GameConfiguration gameConfiguration, Logger logger)
        {
            this.gameConfiguration = gameConfiguration;
            this.logger = logger;
        }
        public void Load()
        {
            var vocations = GetVocations();
            VocationStore.Load(vocations);
            logger.Information($"Vocations loaded!");
        }

        private List<Vocation> GetVocations()
        {
            var jsonString = File.ReadAllText(Path.Combine("./data/vocations.json"));
            var vocations = JsonConvert.DeserializeObject<List<Vocation>>(jsonString, new JsonSerializerSettings
            {

                Converters =
                {
                    new AbstractConverter<VocationFormula, IVocationFormula>(),
                    //new AbstractConverter<VocationSkill, IVocationSkill>(),
                    new SkillConverter()

                }
            });
                //Error = (se, ev) => { ev.ErrorContext.Handled = true; Console.WriteLine(ev.ErrorContext.Error); } });
            return vocations;
        }

        public class SkillConverter : JsonConverter<Dictionary<byte, float>>
        {
            public override Dictionary<byte, float> ReadJson(JsonReader reader, Type objectType, [AllowNull] Dictionary<byte, float> existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
               return serializer.Deserialize<List<Dictionary<string,string>>>(reader).ToDictionary(x=>byte.Parse(x["id"]), x => float.Parse(x["multiplier"], CultureInfo.InvariantCulture.NumberFormat));
            }

            public override void WriteJson(JsonWriter writer, [AllowNull] Dictionary<byte, float> value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
  
}
