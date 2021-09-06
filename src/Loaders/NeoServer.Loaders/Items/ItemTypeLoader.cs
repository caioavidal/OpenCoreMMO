using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.DataStore;
using NeoServer.Loaders.Items.Parsers;
using NeoServer.OTB.Parsers;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json;
using Serilog.Core;

namespace NeoServer.Loaders.Items
{
    public class ItemTypeLoader
    {
        private readonly Logger logger;
        private readonly ServerConfiguration serverConfiguration;
        private readonly ItemTypeStore _itemTypeStore;

        public ItemTypeLoader(Logger logger, ServerConfiguration serverConfiguration, ItemTypeStore itemTypeStore)
        {
            this.logger = logger;
            this.serverConfiguration = serverConfiguration;
            _itemTypeStore = itemTypeStore;
        }

        /// <summary>
        ///     Loads the OTB and XML files into a collection of ItemType objects
        /// </summary>
        public void Load()
        {
            logger.Step("Loading items", "{n} items loaded", () =>
            {
                var basePath = $"{serverConfiguration.Data}/items/";
                var itemTypes = LoadOTB(basePath);

                LoadItemsJson(basePath, itemTypes);

                foreach (var item in itemTypes)
                {
                    _itemTypeStore.Add(item.Key, item.Value);
                    ItemIdMapStore.Data.Add(item.Value.ClientId, item.Key);

                    if (item.Value.Attributes.GetAttribute(ItemAttribute.Type)
                            ?.Equals("coin", StringComparison.InvariantCultureIgnoreCase) ??
                        false) CoinTypeStore.Data.Add(item.Key, item.Value);
                }

                return new object[] {itemTypes.Count};
            });
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

                    while (id <= metadata.Toid) itemTypeMetadataParser.AddMetadata(metadata, id++);
                }
            });
        }
    }
}