using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Items.Containers;
using NeoServer.Game.Items.Items.UsableItems;
using NeoServer.Game.Items.Items.UsableItems.Runes;
using NeoServer.Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Items
{
    public class ItemFactory : IItemFactory, IFactory
    {
        public event CreateItem OnItemCreated;
        public ItemFactory()
        {
            Instance = this;
        }

        public static IItemFactory Instance { get; private set; }

        public IEnumerable<IItemEventSubscriber> ItemEventSubscribers { get; set; }

        public IItem Create(ushort typeId, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {
            if (!ItemTypeData.InMemory.TryGetValue(typeId, out var itemType)) return null;

            var createdItem = CreateItem(itemType, location, attributes);

            SubscribeEvents(createdItem);

            return createdItem;
        }

        private void SubscribeEvents(IItem createdItem)
        {
            foreach (var gameSubscriber in ItemEventSubscribers.Where(x => x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //register game events first
            {
                gameSubscriber.Subscribe(createdItem);
            }

            foreach (var subscriber in ItemEventSubscribers.Where(x => !x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //than register server events
            {
                subscriber.Subscribe(createdItem);
            }
        }

        public IItem Create(string name, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {
            var item = ItemTypeData.InMemory.Values.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (item is null) return null;

            return Create(item.TypeId, location, attributes);
        }

        private IItem CreateItem(IItemType itemType, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {
            if (itemType is null || itemType.TypeId < 100) return null;

            if (itemType.Group == ItemGroup.ITEM_GROUP_DEPRECATED)
            {
                return null;
            }

            if (itemType.Attributes.GetAttribute(ItemAttribute.Script) is string script && !string.IsNullOrWhiteSpace(script))
            {
                var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).FirstOrDefault(x => x.Name.Equals(script));

                if (type is not null && Activator.CreateInstance(type, itemType, location, attributes) is IItem instance) return instance;
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
                if (Rune.IsApplicable(itemType))
                {
                    if (AttackRune.IsApplicable(itemType))
                    {
                        return new AttackRune(itemType, location, attributes);
                    }
                    if (FieldRune.IsApplicable(itemType))
                    {
                        return new FieldRune(itemType, location, attributes);
                    }
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
                    return new TransformerUsableItem(itemType, location);
                }

                //return new UseableOnItem(itemType, location);
            }

            return new Item(itemType, location);
        }
    }
}
