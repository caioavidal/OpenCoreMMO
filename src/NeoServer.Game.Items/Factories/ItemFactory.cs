using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items;
using NeoServer.Server.Items;
using System;
using System.Collections.Generic;
using NeoServer.Game.Items.Items.Containers;
using NeoServer.Game.Items.Items.UsableItems;
using NeoServer.Game.Items.Events;
using NeoServer.Game.Contracts.Items.Types;

namespace NeoServer.Game.Items
{
    public class ItemFactory : IItemFactory, IFactory
    {
        public event CreateItem OnItemCreated;
        private readonly ItemUsedEventHandler itemUsedEventHandler;

        public ItemFactory(ItemUsedEventHandler itemUsedEventHandler)
        {
            this.itemUsedEventHandler = itemUsedEventHandler;
        }

        public IItem Create(ushort typeId, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {
            var createdItem = CreateItem(typeId, location, attributes);
            if(createdItem is IConsumable consumable)
            {
                consumable.OnUsed += (usedBy, creature, item) => itemUsedEventHandler.Execute(this, usedBy, creature, item);
            }
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

            if (Depot.IsApplicable(itemType))
            {
                return new Depot(itemType, location);
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
            if (Cumulative.IsApplicable(itemType))
            {
                if (ThrowableDistanceWeapon.IsApplicable(itemType))
                {
                    return new ThrowableDistanceWeapon(itemType, location, attributes);
                }
                if (AmmoItem.IsApplicable(itemType))
                {
                    return new AmmoItem(itemType, location, attributes);
                }
                if (HealingItem.IsApplicable(itemType))
                {
                    return new HealingItem(itemType, location, attributes);
                }
                if (Food.IsApplicable(itemType))
                {
                    return new Food(itemType, location, attributes);
                }
                return new Cumulative(itemType, location, attributes);
            }
            if (LiquidPoolItem.IsApplicable(itemType))
            {
                return new LiquidPoolItem(itemType, location, attributes);
            }
            if (MagicField.IsApplicable(itemType))
            {
                return new MagicField(itemType, location);
            }
            if (FloorChanger.IsApplicable(itemType))
            {
                return new FloorChanger(itemType, location);
            }
            if (UseableOnItem.IsApplicable(itemType))
            {
                if (FloorChangerUsableItem.IsApplicable(itemType))
                {
                    return new FloorChangerUsableItem(itemType, location);
                }
                if (TransformerUsableItem.IsApplicable(itemType))
                {
                    return new TransformerUsableItem(itemType, location, this); 
                }
           
                return new UseableOnItem(itemType, location);
            }

            return new Item(ItemTypeData.InMemory[typeId], location);
        }
    }
}
