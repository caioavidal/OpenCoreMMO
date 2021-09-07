using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.DataStore;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Items.Containers;
using NeoServer.Game.Items.Items.Protections;
using NeoServer.Game.Items.Items.UsableItems;
using NeoServer.Game.Items.Items.UsableItems.Runes;
using NeoServer.Game.Items.Items.Weapons;
using CreateItem = NeoServer.Game.Common.Contracts.Items.CreateItem;

namespace NeoServer.Game.Items.Factories
{
    public class ItemFactory : IItemFactory, IFactory
    {
        public ItemFactory()
        {
            Instance = this;
        }

        public static IItemFactory Instance { get; private set; }

        public IEnumerable<IItemEventSubscriber> ItemEventSubscribers { get; set; }
        public event CreateItem OnItemCreated;
        public AccessoryFactory AccessoryFactory { get; set; }
        public ItemTypeStore ItemTypeStore { get; set; }


        public IItem CreateLootCorpse(ushort typeId, Location location, ILoot loot)
        {
            if (!ItemTypeStore.TryGetValue(typeId, out var itemType)) return null;

            var createdItem = new LootContainer(itemType, location, loot);

            SubscribeEvents(createdItem);

            return createdItem;
        }

        public IItem Create(ushort typeId, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {
            if (!ItemTypeStore.TryGetValue(typeId, out var itemType)) return null;

            var createdItem = CreateItem(itemType, location, attributes);

            SubscribeEvents(createdItem);

            return createdItem;
        }
        public IItem Create(IItemType itemType, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {
            var createdItem = CreateItem(itemType, location, attributes);

            SubscribeEvents(createdItem);

            return createdItem;
        }


        public IEnumerable<ICoin> CreateCoins(ulong amount)
        {
            var coinsToAdd = CoinCalculator.Calculate(CoinTypeStore.Data.Map, amount);

            foreach (var coinToAdd in coinsToAdd)
            {
                var createdCoin = Create(coinToAdd.Item1, Location.Inventory(Slot.Backpack), null);
                if (createdCoin is not ICoin newCoin) continue;
                newCoin.Amount = coinToAdd.Item2;

                yield return newCoin;
            }
        }

        public IItem Create(string name, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {
            var item = ItemTypeStore.All.FirstOrDefault(x =>
                x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (item is null) return null;

            return Create(item.TypeId, location, attributes);
        }

        private void SubscribeEvents(IItem createdItem)
        {
            foreach (var gameSubscriber in ItemEventSubscribers.Where(x =>
                x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //register game events first
                gameSubscriber.Subscribe(createdItem);

            foreach (var subscriber in ItemEventSubscribers.Where(x =>
                !x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //than register server events
                subscriber.Subscribe(createdItem);
        }

        private IItem CreateItem(IItemType itemType, Location location,
            IDictionary<ItemAttribute, IConvertible> attributes)
        {
            if (itemType is null || itemType.TypeId < 100) return null;

            if (itemType.Group == ItemGroup.ITEM_GROUP_DEPRECATED) return null;

            if (itemType.Attributes.GetAttribute(ItemAttribute.Script) is string script &&
                !string.IsNullOrWhiteSpace(script))
            {
                var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .FirstOrDefault(x => x.Name.Equals(script));

                if (type is not null &&
                    Activator.CreateInstance(type, itemType, location, attributes) is IItem instance) return instance;
            }

            if (GroundItem.IsApplicable(itemType)) return new GroundItem(itemType, location);
            if (MeleeWeapon.IsApplicable(itemType)) return new MeleeWeapon(itemType, location);
            if (DistanceWeapon.IsApplicable(itemType)) return new DistanceWeapon(itemType, location);

            if (Depot.IsApplicable(itemType)) return new Depot(itemType, location);

            if (PickupableContainer.IsApplicable(itemType)) return new PickupableContainer(itemType, location);
            if (Container.IsApplicable(itemType)) return new Container(itemType, location);
            if (BodyDefenseEquimentItem.IsApplicable(itemType)) return new BodyDefenseEquimentItem(itemType, location);

            if(AccessoryFactory.Create(itemType, location) is Accessory accessory) return accessory;
            
            if (MagicWeapon.IsApplicable(itemType)) return new MagicWeapon(itemType, location);
            if (ICumulative.IsApplicable(itemType))
            {
                if (Coin.IsApplicable(itemType)) return new Coin(itemType, location, attributes);
                if (ThrowableDistanceWeapon.IsApplicable(itemType))
                    return new ThrowableDistanceWeapon(itemType, location, attributes);
                if (AmmoItem.IsApplicable(itemType)) return new AmmoItem(itemType, location, attributes);
                if (HealingItem.IsApplicable(itemType)) return new HealingItem(itemType, location, attributes);
                if (Food.IsApplicable(itemType)) return new Food(itemType, location, attributes);
                if (Rune.IsApplicable(itemType))
                {
                    if (AttackRune.IsApplicable(itemType)) return new AttackRune(itemType, location, attributes);
                    if (FieldRune.IsApplicable(itemType)) return new FieldRune(itemType, location, attributes);
                }

                return new Cumulative(itemType, location, attributes);
            }

            if (LiquidPoolItem.IsApplicable(itemType)) return new LiquidPoolItem(itemType, location, attributes);
            if (MagicField.IsApplicable(itemType)) return new MagicField(itemType, location);
            if (FloorChanger.IsApplicable(itemType)) return new FloorChanger(itemType, location);
            if (UseableOnItem.IsApplicable(itemType))
            {
                if (FloorChangerUsableItem.IsApplicable(itemType))
                    return new FloorChangerUsableItem(itemType, location);
                if (TransformerUsableItem.IsApplicable(itemType)) return new TransformerUsableItem(itemType, location);

                //return new UseableOnItem(itemType, location);
            }

            return new Item(itemType, location);
        }
    }
}