using NeoServer.Server.Helpers;
using NeoServer.Server.Items;
using NeoServer.Server.Model.Items;
using NeoServer.Server.World.OTB;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace NeoServer.Server.Loaders
{
    public class ItemLoader
    {

        public static void Load() => ItemData.Load(LoadItems());

        private static Dictionary<ushort, ItemType> LoadItems()
        {
            var itemDictionary = new Dictionary<UInt16, ItemType>();

            var attrsNotSuported = 0;
            var attrsNotValid = 0;

            var fileTree = OTBDeserializer.DeserializeOTBData(new ReadOnlyMemory<byte>(ServerResourcesManager.GetItemsBytes("items.otb"))); //todo
            foreach (var itemChildren in fileTree.Children)
            {
                var current = new ItemType();
                var itemStream = new OTBParsingStream(itemChildren.Data);

                var flags = itemStream.ReadUInt32();
                current.ParseOTFlags(flags);

                while (!itemStream.IsOver)
                {
                    var attr = itemStream.ReadByte();
                    var dataSize = itemStream.ReadUInt16();

                    switch ((ItemAttributes)attr)
                    {
                        case ItemAttributes.ServerId:
                            var serverId = itemStream.ReadUInt16();

                            if (serverId == 4535)
                            {
                                serverId = 4535;
                            }

                            if (serverId > 30000 && serverId < 30100)
                                serverId -= 30000;

                            current.SetId(serverId);
                            break;

                        case ItemAttributes.ClientId:
                            current.SetClientId(itemStream.ReadUInt16());
                            break;

                        default:
                            itemStream.Skip(dataSize);
                            break;
                    }
                }
                itemDictionary.Add(current.TypeId, current);
            }

            var rootElement = XElement.Load(ServerResourcesManager.GetItems("items.xml"), LoadOptions.SetLineInfo);

            foreach (var element in rootElement.Elements("item"))
            {
                var id = element.Attribute("id");
                var fromId = element.Attribute("fromid");
                var toId = element.Attribute("toid");

                // Malformed element, missing id information, ignore it
                if (id == null && (fromId == null || toId == null))
                    continue;

                ushort serverId = 0;
                ushort aplyTo = 1;
                if (id == null)
                {
                    // Ignore if can't parse the values or if fromId >= toId
                    if (!ushort.TryParse(fromId.Value, out serverId) || !ushort.TryParse(toId.Value, out aplyTo) || serverId >= aplyTo)
                        continue;

                    aplyTo -= serverId;
                }
                else
                {
                    if (!ushort.TryParse(id.Value, out serverId))
                        continue;
                }

                for (ushort key = serverId; key < serverId + aplyTo; key++)
                {
                    if (!itemDictionary.TryGetValue(key, out ItemType current))
                        continue;

                    var name = element.Attribute("name");
                    if (name != null)
                        current.SetName(name.Value);

                    foreach (var attribute in element.Elements("attribute"))
                    {
                        var attrName = attribute.Attribute("key");
                        var attrValue = attribute.Attribute("value");

                        if (attrName == null || attrValue == null)
                            continue;

                        if (attrName.Value == "description")
                        {
                            current.SetDescription(attrValue.Value);
                            continue;
                        }

                        var lineInfo = (IXmlLineInfo)attribute;
                        var attr = OpenTibiaTranslationMap.TranslateAttributeName(attrName.Value, out bool success);

                        if (success)
                        {
                            int value = -1;
                            bool setAttr = true;
                            switch (attrName.Value)
                            {
                                case "weaponType":
                                    success = current.ParseOTWeaponType(attrValue.Value);
                                    setAttr = false;
                                    break;

                                case "fluidSource":
                                    value = OpenTibiaTranslationMap.TranslateLiquidType(attrValue.Value, out success);
                                    break;

                                case "corpseType":
                                    value = OpenTibiaTranslationMap.TranslateCorpseType(attrValue.Value, out success);
                                    break;

                                case "slotType":
                                    value = OpenTibiaTranslationMap.TranslateSlotType(attrValue.Value, out success);
                                    break;

                                default:
                                    success = int.TryParse(attrValue.Value, out value);
                                    break;
                            }

                            if (!success)
                            {
                                attrsNotValid++;
                                //Console.WriteLine($"[{Path.GetFileName(itemExtensionFilePath)}:{lineInfo.LineNumber}] \"{attrValue.Value}\" is not a valid value for attribute \"{attrName.Value}\"");
                            }
                            else if (setAttr)
                                current.SetAttribute(attr, value);

                        }
                        else
                        {
                            attrsNotSuported++;
                            //Console.WriteLine($"[{Path.GetFileName(itemExtensionFilePath)}:{lineInfo.LineNumber}] Attribute \"{attrName.Value}\" is not supported!");
                        }
                    }

                }
            }

            foreach (var type in itemDictionary)
            {
                type.Value.LockChanges();
            }

            Console.WriteLine($"Items with attributes not supported: {attrsNotSuported}");
            Console.WriteLine($"Not valid attributes: {attrsNotSuported}");

            return itemDictionary;
        }

    }
    public enum ItemAttributes : byte
    {
        ServerId = 0x10,
        ClientId,
        Name,
        Description,
        Speed,
        Slot,
        MaxItems,
        Weight,
        Weapon,
        Ammunition,
        Armor,
        MagicLevel,
        MagicFieldType,
        Writeable,
        RotateTo,
        Decay,
        SpriteHash,
        MiniMapColor,
        Attr07,
        Attr08,
        Light,

        //1-byte aligned
        Decay2, //deprecated
        Weapon2, //deprecated
        Ammunition2, //deprecated
        Armor2, //deprecated
        Writeable2, //deprecated
        Light2,
        TopOrder,
        Writeable3, //deprecated

        WareId
    }
}
