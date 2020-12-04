using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Vocations;
using NeoServer.Server.Helpers.JsonConverters;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Generic;
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
            var vocations = JsonConvert.DeserializeObject<List<Vocation>>(jsonString, new JsonSerializerSettings {

                Converters =
                {
                    new AbstractConverter<VocationFormula, IVocationFormula>(),
                    new AbstractConverter<VocationSkill, IVocationSkill>(),
                },
                Error = (se, ev) => { ev.ErrorContext.Handled = true; Console.WriteLine(ev.ErrorContext.Error); } });
            return vocations;
        }
      
    }
  
}
