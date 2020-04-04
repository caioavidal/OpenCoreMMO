using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoServer.Game.Contracts.Item;
using NeoServer.OTB.Parsers;
using NeoServer.Server.Items;
using Newtonsoft.Json;

namespace NeoServer.Loaders.Items
{
    public class ItemTypeLoader
    {
        /// <summary>
        /// Loads the OTB and XML files into a collection of ItemType objects
        /// </summary>
        public void Load()
        {
            var basePath = "/home/caio/sources/neoserver/data/items/";
            var itemTypes = LoadOTB(basePath);
            LoadItemsJson(basePath, itemTypes);

            ItemTypeData.Load(itemTypes);

            Console.WriteLine($"{itemTypes.Count} items loaded");
        }

        private Dictionary<ushort, IItemType> LoadOTB(string basePath)
        {
            var fileStream = File.ReadAllBytes($"{basePath}/items.otb");
            var otbNode = OTBBinaryTreeBuilder.Deserialize(fileStream);
            var otb = new OTB.Structure.OTB(otbNode);

            var itemTypes = otb.ItemNodes.Select(i => ItemNodeParser.Parse(i)).ToDictionary(x => x.TypeId);
            return itemTypes;
        }

        private void LoadItemsJson(string basePath, Dictionary<ushort, IItemType> itemTypes)
        {
            var itemTypeMetadatas = JsonConvert.DeserializeObject<IEnumerable<ItemTypeMetadata>>(File.ReadAllText($"{basePath}/items.json"));


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
                    Console.WriteLine("[Warning - Items::loadFromXml] No item id found");
                    continue;
                }

                if (metadata.Toid == null)
                {
                    Console.WriteLine($"[Warning - Items::loadFromXml] fromid ({metadata.Fromid}) without toid");
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