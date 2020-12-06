using NeoServer.Game.Contracts.Items;
using NeoServer.OTB.Parsers;
using NeoServer.Server.Items;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoServer.Loaders.Items
{
    public class ItemTypeLoader
    {
        private readonly Logger logger;
        public ItemTypeLoader(Logger logger)
        {
            this.logger = logger;
        }
        /// <summary>
        /// Loads the OTB and XML files into a collection of ItemType objects
        /// </summary>
        public void Load()
        {

            var basePath = "./data/items/";
            var itemTypes = LoadOTB(basePath);

            LoadItemsJson(basePath, itemTypes);
            ItemTypeData.Load(itemTypes);
            logger.Information($"{itemTypes.Count} items loaded");
        }

        private Dictionary<ushort, IItemType> LoadOTB(string basePath)
        {
            var fileStream = File.ReadAllBytes(Path.Combine(basePath, "items.otb"));
            var otbNode = OTBBinaryTreeBuilder.Deserialize(fileStream);
            var otb = new OTB.Structure.OTB(otbNode);
            var itemTypes = otb.ItemNodes.AsParallel().Select(i => ItemNodeParser.Parse(i)).ToDictionary(x => x.TypeId);
            return itemTypes;
        }

        private void LoadItemsJson(string basePath, Dictionary<ushort, IItemType> itemTypes)
        {
            var jsonString = File.ReadAllText(Path.Combine(basePath, "items.json"));
            var itemTypeMetadatas = JsonConvert.DeserializeObject<IEnumerable<ItemTypeMetadata>>(jsonString);

            var itemTypeMetadataParser = new ItemTypeMetadataParser(itemTypes);
            foreach (var metadata in itemTypeMetadatas)
            {
                if (metadata.Id.HasValue)
                {
                    itemTypeMetadataParser.AddMetadata(metadata, metadata.Id.Value);
                    continue;
                }

                if (metadata.Fromid == null)
                {
                    Console.WriteLine("No item id found");
                    continue;
                }

                if (metadata.Toid == null)
                {
                    Console.WriteLine($"fromid ({metadata.Fromid}) without toid");
                    continue;
                }

                var id = metadata.Fromid.Value;

                while (id <= metadata.Toid)
                {
                    itemTypeMetadataParser.AddMetadata(metadata, id++);
                }
            }
        }
    }
}