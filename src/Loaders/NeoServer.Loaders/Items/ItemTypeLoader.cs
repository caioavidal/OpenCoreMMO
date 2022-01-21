using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoServer.Data.InMemory.DataStores;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Loaders.Items.Parsers;
using NeoServer.OTB.Parsers;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json;
using Serilog;

namespace NeoServer.Loaders.Items
{
    public class ItemTypeLoader
    {
        private readonly ILogger _logger;
        private readonly ServerConfiguration _serverConfiguration;
        private readonly IItemTypeStore _itemTypeStore;

        public ItemTypeLoader(ILogger logger, ServerConfiguration serverConfiguration, IItemTypeStore itemTypeStore)
        {
            _logger = logger;
            _serverConfiguration = serverConfiguration;
            _itemTypeStore = itemTypeStore;
        }

        /// <summary>
        ///     Loads the OTB and XML files into a collection of ItemType objects
        /// </summary>
        public void Load()
        {
            _logger.Step("Loading items", "{n} items loaded", () =>
            {
                var basePath = $"{_serverConfiguration.Data}/items/";
                var itemTypes = LoadOtb(basePath);

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

        private Dictionary<ushort, IItemType> LoadOtb(string basePath)
        {
            var fileStream = File.ReadAllBytes(Path.Combine(basePath, "items.otb"));
            var otbNode = OTBBinaryTreeBuilder.Deserialize(fileStream);
            var otb = new OTB.Structure.OTB(otbNode);
            var itemTypes = otb.ItemNodes.AsParallel().Select(ItemNodeParser.Parse).ToDictionary(x => x.TypeId);
            return itemTypes;
        }

        private static void LoadItemsJson(string basePath, IDictionary<ushort, IItemType> itemTypes)
        {
            var jsonString = File.ReadAllText(Path.Combine(basePath, "items.json"));

            var itemTypeMetadata = JsonConvert.DeserializeObject<IEnumerable<ItemTypeMetadata>>(jsonString);

            var itemTypeMetadataParser = new ItemTypeMetadataParser(itemTypes);

            (itemTypeMetadata ?? Array.Empty<ItemTypeMetadata>()).AsParallel().ForAll(metadata =>
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