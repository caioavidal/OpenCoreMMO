using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Items.Items;
using NeoServer.Server.Items;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Items
{
    public class ItemFactory : IItemFactory, IFactory
    {
        public event CreateItem OnItemCreated;

        public IItem Create(ushort typeId, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {
            var createdItem = CreateItem(typeId, location, attributes);
            return createdItem;
        }

        private IItem CreateItem(ushort typeId, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
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
            if (MeleeWeapon.IsApplicable(itemType))
            {
                return new MeleeWeapon(itemType, location);
            }
            if (DistanceWeapon.IsApplicable(itemType))
            {
                return new DistanceWeapon(itemType, location);
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
            if (Wand.IsApplicable(itemType))
            {
                return new Wand(itemType, location);
            }
            if (CumulativeItem.IsApplicable(itemType))
            {
                if (ThrowableDistanceWeapon.IsApplicable(itemType))
                {
                    return new ThrowableDistanceWeapon(itemType, location, attributes);
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
