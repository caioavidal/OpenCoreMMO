using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Items.Items;
using NeoServer.Server.Items;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Items
{

    public class ItemFactory : IItemFactory
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
            if (DistanceWeaponItem.IsApplicable(itemType))
            {
                return new DistanceWeaponItem(itemType, location);
            }

            if (PickupableContainer.IsApplicable(itemType))
            {
                return new PickupableContainer(itemType, location);
            }
            if (Container.IsApplicable(itemType))
            {
                return new Container(itemType, location);
            }
            if (BodyDefenseEquimentItem.IsApplicable(itemType))
            {
                return new BodyDefenseEquimentItem(itemType, location);
            }
            if (Ring.IsApplicable(itemType))
            {
                return new Ring(itemType, location);
            }
            if (Necklace.IsApplicable(itemType))
            {
                return new Necklace(itemType, location);
            }
            if (CumulativeItem.IsApplicable(itemType))
            {
                if (ThrowableDistanceWeaponItem.IsApplicable(itemType))
                {
                    return new ThrowableDistanceWeaponItem(itemType, location, attributes);
                }
                if (AmmoItem.IsApplicable(itemType))
                {
                    return new AmmoItem(itemType, location, attributes);
                }
                return new CumulativeItem(itemType, location, attributes);
            }
            if (LiquidPoolItem.IsApplicable(itemType))
            {
                return new LiquidPoolItem(itemType, location, attributes);
            }
            if (MagicField.IsApplicable(itemType))
            {
                return new MagicField(itemType, location);
            }

            //throw new NotImplementedException("Item not handled");
            //return new BaseItem(itemType.Name, itemType.ClientId);
            return new Item(ItemTypeData.InMemory[typeId], location);
        }
    }
}
