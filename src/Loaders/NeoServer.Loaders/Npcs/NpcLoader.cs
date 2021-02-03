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
        public event Action<INpcType, string> OnLoad;
        public void Load()
        {
            var npcs = ConvertNpcs();

            foreach (var npcLoaded in npcs)
            {
                var (jsonContent, npc) = npcLoaded;
                NpcStore.Data.Add(npc.Name, npc);
                OnLoad?.Invoke(npc, jsonContent);
            }

            logger.Information("{n} NPCs loaded!", npcs.Count());
        }
        private IEnumerable<(string,INpcType)> ConvertNpcs()
        {
            var basePath = $"{serverConfiguration.Data}/npcs";
            foreach (var file in Directory.GetFiles(basePath, "*.json"))
            {
                var jsonContent = File.ReadAllText(file);
                var npc = JsonConvert.DeserializeObject<NpcData>(jsonContent, new JsonSerializerSettings { Error = (se, ev) => { ev.ErrorContext.Handled = true; Console.WriteLine(ev.ErrorContext.Error); } });

                if (npc is null) continue;
                if (string.IsNullOrWhiteSpace(npc.Name)) continue;


                var dialogs = new List<INpcDialog>();

                foreach (var dialog in npc.Dialog)
                {
                    dialogs.Add(ConvertDialog(dialog));
                }

                yield return (jsonContent, new NpcType()
                {
                    Script = npc.Script,
                    MaxHealth = npc.Health?.Max ?? 100,
                    Name = npc.Name,
                    Speed = 280,
                    Look = new Dictionary<LookType, ushort>() { { LookType.Type, npc.Look.Type }, { LookType.Corpse, npc.Look.Corpse }, { LookType.Body, npc.Look.Body}, { LookType.Legs, npc.Look.Legs}, { LookType.Head, npc.Look.Head },
                { LookType.Feet, npc.Look.Feet},{ LookType.Addon, npc.Look.Addons}},
                    Dialog = dialogs.ToArray()
                });
            }

        }

        private static INpcDialog ConvertDialog(NpcData.DialogData dialog)
        {
            if (dialog is null) return null;
            var d = new NpcDialogType
            {
                Answers = dialog.Answers,
                Action = dialog.Action,
                OnWords = dialog.OnWords,
                End = dialog.End,
                Then = dialog.Then?.Select(x=> ConvertDialog(x))?.ToArray() ?? null
            };
            return d;
        }
    }

   
}
