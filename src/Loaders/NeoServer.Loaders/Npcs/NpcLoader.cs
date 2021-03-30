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
        private IEnumerable<(string, INpcType)> ConvertNpcs()
        {
            var basePath = $"{serverConfiguration.Data}/npcs";
            foreach (var file in Directory.GetFiles(basePath, "*.json"))
            {
                var jsonContent = File.ReadAllText(file);
                var npcData = JsonConvert.DeserializeObject<NpcJsonData>(jsonContent, new JsonSerializerSettings { Error = (se, ev) => { ev.ErrorContext.Handled = true; Console.WriteLine(ev.ErrorContext.Error); } });

                if (npcData is null) continue;
                if (string.IsNullOrWhiteSpace(npcData.Name)) continue;

                var dialogs = new List<IDialog>();

                foreach (var dialog in npcData.Dialog)
                {
                    dialogs.Add(ConvertDialog(dialog));
                }

                var npcType = (jsonContent, new NpcType()
                {
                    Script = npcData.Script,
                    MaxHealth = npcData.Health?.Max ?? 100,
                    Name = npcData.Name,
                    Marketings = npcData.Marketings,
                    Speed = 280,
                    Look = new Dictionary<LookType, ushort>() { { LookType.Type, npcData.Look.Type }, { LookType.Corpse, npcData.Look.Corpse }, { LookType.Body, npcData.Look.Body}, { LookType.Legs, npcData.Look.Legs}, { LookType.Head, npcData.Look.Head },
                { LookType.Feet, npcData.Look.Feet},{ LookType.Addon, npcData.Look.Addons}},
                    Dialogs = dialogs.ToArray()
                });

                LoadShopData(npcType.Item2, npcData);

                NpcCustomAttributeLoader.LoadCustomData(npcType.Item2, npcData);

                yield return npcType;
            }
        }

        private void LoadShopData(INpcType type, NpcJsonData npcData)
        {
            if (type is null || npcData is null || npcData.Shop is null) return;

            var items = new Dictionary<ushort, IShopItem>(npcData.Shop.Length);
            foreach (var item in npcData.Shop)
            {
                if (!ItemTypeStore.Data.TryGetValue(item.Item, out var itemType)) continue;
                items.Add(itemType.TypeId, new ShopItem(itemType, item.Buy, item.Sell));
            }

            type.CustomAttributes.Add("shop", items);
        }

        private IDialog ConvertDialog(NpcJsonData.DialogData dialog)
        {
            if (dialog is null) return null;
            var d = new Dialog
            {
                Back = dialog.Back,
                Answers = dialog.Answers,
                Action = dialog.Action,
                OnWords = dialog.OnWords,
                End = dialog.End,
                StoreAt = dialog.StoreAt,
                Then = dialog.Then?.Select(x => ConvertDialog(x))?.ToArray() ?? null
            };
            return d;
        }
    }

}
