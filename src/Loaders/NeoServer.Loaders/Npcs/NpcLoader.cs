using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Npcs;
using NeoServer.Game.DataStore;
using NeoServer.Loaders.Interfaces;
using NeoServer.Server.Standalone;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Loaders.Npcs
{
    public class NpcLoader : IStartupLoader
    {
        private readonly ServerConfiguration serverConfiguration;
        private readonly Logger logger;

        public NpcLoader(ServerConfiguration serverConfiguration, Logger logger)
        {
            this.serverConfiguration = serverConfiguration;
            this.logger = logger;
        }

        public void Load()
        {
            var npcs = ConvertNpcs();

            foreach (var npc in npcs) NpcStore.Data.Add(npc.Name, npc);
            
            logger.Information("{n} NPCs loaded!", npcs.Count());
        }
        private IEnumerable<INpcType> ConvertNpcs()
        {
            var basePath = $"{serverConfiguration.Data}/npcs";
            foreach (var file in Directory.GetFiles(basePath, "*.json"))
            {
                var jsonContent = File.ReadAllText(file);
                var npc = JsonConvert.DeserializeObject<NpcData>(jsonContent, new JsonSerializerSettings { Error = (se, ev) => { ev.ErrorContext.Handled = true; Console.WriteLine(ev.ErrorContext.Error); } });

                if (npc is null) continue;
                if (string.IsNullOrWhiteSpace(npc.Name)) continue;

                yield return new NpcType()
                {
                    MaxHealth = npc.Health?.Max ?? 100,
                    Name = npc.Name,
                    Speed = 280,
                    Look = new Dictionary<LookType, ushort>() { { LookType.Type, npc.Look.Type }, { LookType.Corpse, npc.Look.Corpse }, { LookType.Body, npc.Look.Body}, { LookType.Legs, npc.Look.Legs}, { LookType.Head, npc.Look.Head },
                { LookType.Feet, npc.Look.Feet},{ LookType.Addon, npc.Look.Addons}},
                };
            }

        }
    }

    public class NpcData
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("walk-interval")]
        public int WalkInterval { get; set; }
        [JsonProperty("health")]
        public HealthData Health { get; set; }
        [JsonProperty("look")]
        public LookData Look { get; set; }
        public class HealthData
        {
            [JsonProperty("now")]
            public uint Now { get; set; }

            [JsonProperty("max")]
            public uint Max { get; set; }
        }

        public class LookData
        {
            [JsonProperty("type")]
            public ushort Type { get; set; }

            [JsonProperty("corpse")]
            public ushort Corpse { get; set; }
            [JsonProperty("body")]
            public ushort Body { get; set; }
            [JsonProperty("legs")]
            public ushort Legs { get; set; }
            [JsonProperty("feet")]
            public ushort Feet { get; set; }
            [JsonProperty("head")]
            public ushort Head { get; set; }
            [JsonProperty("addons")]
            public ushort Addons { get; set; }
        }
    }
}
