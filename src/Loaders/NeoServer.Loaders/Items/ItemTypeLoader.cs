using NeoServer.Game.Contracts.Items;
using NeoServer.OTB.Parsers;
using NeoServer.Server.Items;
using NeoServer.Server.Standalone;
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
        private readonly ServerConfiguration serverConfiguration;
        public ItemTypeLoader(Logger logger, ServerConfiguration serverConfiguration)
        {
            this.logger = logger;
            this.serverConfiguration = serverConfiguration;
        }
        /// <summary>
        /// Loads the OTB and XML files into a collection of ItemType objects
        /// </summary>
        public void Load()
        {
            var basePath = $"{serverConfiguration.Data}/items/";
            var itemTypes = LoadOTB(basePath);

            LoadItemsJson(basePath, itemTypes);
            ItemTypeData.Load(itemTypes);
            logger.Information("{n} items loaded", itemTypes.Count);
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

            itemTypeMetadatas.AsParallel().ForAll(metadata =>
            {
                if (metadata.Id.HasValue)
                {
                    itemTypeMetadataParser.AddMetadata(metadata, metadata.Id.Value);
                }
                else if (metadata.Fromid == null)
                {
                    Console.WriteLine("No item id found");
                }
                else if (metadata.Toid == null)
                {
                    Console.WriteLine($"fromid ({metadata.Fromid}) without toid");
                }
                else
                {
                    var id = metadata.Fromid.Value;

                    while (id <= metadata.Toid)
                    {
                        itemTypeMetadataParser.AddMetadata(metadata, id++);
                    }
                }
            });

        }
    }
}