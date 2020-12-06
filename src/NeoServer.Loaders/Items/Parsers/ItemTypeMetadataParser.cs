using NeoServer.Game.Contracts.Items;
using NeoServer.OTB.Parsers;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Loaders.Items
{
    public class ItemTypeMetadataParser
    {

        private IDictionary<ushort, IItemType> itemTypes;

        public ItemTypeMetadataParser(IDictionary<ushort, IItemType> itemTypes)
        {
            this.itemTypes = itemTypes;
        }

        /// <summary>
        /// Parses ItemNode object to IItemType
        /// </summary>
        /// <param name="itemNode"></param>
        /// <returns></returns>
        public void AddMetadata(ItemTypeMetadata metadata, ushort itemTypeId)
        {

            var id = itemTypeId;

            if (id > 30000 && id < 30100)
            {
                id -= 30000;
            }

            if (!itemTypes.TryGetValue(id, out IItemType itemType))
            {
                return;
            }

            itemType.SetName(metadata.Name);
            itemType.SetArticle(metadata.Article);

            
            itemType.SetRequirements(metadata.ItemRequirements);

            itemType.SetPlural(metadata.Plural);

            if (metadata.Flags is not null)
            {
                foreach (var flagName in metadata.Flags)
                {
                    if (!OpenTibiaTranslationMap.TranslateFlagName(flagName, out var flag)) continue;
                    itemType.Flags.Add(flag);
                }
            }

            if (metadata.Attributes == null) return;
            foreach (var attribute in metadata.Attributes)
            {
                var itemAttribute = OpenTibiaTranslationMap.TranslateAttributeName(attribute.Key, out bool success);

                itemType.Attributes.SetAttribute(itemAttribute, attribute.Value);
            }

            if (metadata.OnUse == null) return;
            foreach (var attribute in metadata.OnUse)
            {
                var itemAttribute = OpenTibiaTranslationMap.TranslateAttributeName(attribute.Key, out bool success);
                itemType.SetOnUse();
                itemType.OnUse.SetAttribute(itemAttribute, attribute.Value);
            }
        }
    }
}