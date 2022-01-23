using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Items;
using NeoServer.OTB.Parsers;
using Newtonsoft.Json.Linq;

namespace NeoServer.Loaders.Items.Parsers;

public class ItemTypeMetadataParser
{
    private readonly IDictionary<ushort, IItemType> itemTypes;

    public ItemTypeMetadataParser(IDictionary<ushort, IItemType> itemTypes)
    {
        this.itemTypes = itemTypes;
    }

    /// <summary>
    ///     Parses ItemNode object to IItemType
    /// </summary>
    /// <param name="itemNode"></param>
    /// <returns></returns>
    public void AddMetadata(ItemTypeMetadata metadata, ushort itemTypeId)
    {
        var id = itemTypeId;

        if (id > 30000 && id < 30100) id -= 30000;

        if (!itemTypes.TryGetValue(id, out var itemType)) return;

        itemType.SetName(metadata.Name);
        itemType.SetArticle(metadata.Article);

        itemType.SetPlural(metadata.Plural);

        if (metadata.Flags is not null)
            foreach (var flagName in metadata.Flags)
            {
                if (!ItemAttributeTranslation.TranslateFlagName(flagName, out var flag)) continue;
                itemType.Flags.Add(flag);
            }

        if (metadata.Attributes == null) return;

        SetAttributes(metadata.Attributes, itemType.Attributes);

        if (metadata.OnUse == null) return;
        foreach (var attribute in metadata.OnUse)
        {
            var itemAttribute = ItemAttributeTranslation.Translate(attribute.Key, out var success);
            itemType.SetOnUse();

            if (itemAttribute == ItemAttribute.None)
                itemType.OnUse.SetCustomAttribute(attribute.Key, attribute.Value);
            else
                itemType.OnUse.SetAttribute(itemAttribute, attribute.Value);
        }
    }

    private static void SetAttributes(IEnumerable<ItemTypeMetadata.Attribute> metaAttributes,
        IItemAttributeList attributes)
    {
        foreach (var attribute in metaAttributes)
        {
            var itemAttribute = ItemAttributeTranslation.Translate(attribute.Key, out var success);

            var value = itemAttribute == ItemAttribute.Weight
                ? (int.Parse(attribute.Value) / 100f).ToString()
                : attribute.Value; //todo place this code in another place

            if (attribute.Attributes is null || !attribute.Attributes.Any())
            {
                if (value is JArray jArray)
                {
                    value = jArray.ToObject<string[]>();

                    if (itemAttribute == ItemAttribute.None)
                        attributes.SetCustomAttribute(attribute.Key, values: value);
                    else
                        attributes.SetAttribute(itemAttribute, values: value);
                }
                else
                {
                    if (itemAttribute == ItemAttribute.None)
                        attributes.SetCustomAttribute(attribute.Key, value);
                    else
                        attributes.SetAttribute(itemAttribute, value);
                }
            }
            else
            {
                var innerAttributes = new ItemAttributeList();

                SetAttributes(attribute.Attributes, innerAttributes);

                if (itemAttribute == ItemAttribute.None)
                    attributes.SetCustomAttribute(attribute.Key, value, innerAttributes);
                else
                    attributes.SetAttribute(itemAttribute, value, innerAttributes);
            }
        }
    }
}