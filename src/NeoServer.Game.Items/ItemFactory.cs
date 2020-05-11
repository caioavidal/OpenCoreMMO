using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Items.Items;
using NeoServer.Server.Items;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Items
{

    public class ItemFactory
    {
        public static IItem Create(ushort typeId, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {

            if (typeId < 100 || !ItemTypeData.InMemory.ContainsKey(typeId))
            {
                return null;
            }

            var itemType = ItemTypeData.InMemory[typeId];

            if (itemType.Group == ItemGroup.ITEM_GROUP_DEPRECATED)
            {
                return null;
            }

            if (GroundItem.IsApplicable(itemType))
            {
                return new GroundItem(itemType, location);
            }
            if (WeaponItem.IsApplicable(itemType))
            {
                return new WeaponItem(itemType, location);
            }
            if (Container.IsApplicable(itemType))
            {
                return new Container(itemType, location);
            }
            if (BodyDefenseEquimentItem.IsApplicable(itemType))
            {
                return new BodyDefenseEquimentItem(itemType, location);
            }
            if (CumulativeItem.IsApplicable(itemType))
            {
                return new CumulativeItem(itemType, location, attributes);
            }
            if (LiquidPoolItem.IsApplicable(itemType))
            {
                return new LiquidPoolItem(itemType, location, attributes);
            }
            if (MagicFieldItem.IsApplicable(itemType))
            {
                return new MagicFieldItem(itemType, location);
            }

            //throw new NotImplementedException("Item not handled");
            //return new BaseItem(itemType.Name, itemType.ClientId);
            return new Item(ItemTypeData.InMemory[typeId], location);
        }
    }
}
