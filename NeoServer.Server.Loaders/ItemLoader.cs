using NeoServer.Server.Model.Items;
using NeoServer.Server.World.OTB;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Loaders
{
    public class ItemLoader
    {
        public Dictionary<ushort, ItemType> LoadItems()
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
