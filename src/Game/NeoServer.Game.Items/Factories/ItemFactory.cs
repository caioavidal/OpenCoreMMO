using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Items.Containers;
using NeoServer.Game.Items.Items.UsableItems;
using CreateItem = NeoServer.Game.Common.Contracts.Items.CreateItem;

namespace NeoServer.Game.Items.Factories
{
    public class ItemFactory : IItemFactory
    {
        public ItemFactory()
        {
            Instance = this;
        }

        public static IItemFactory Instance { get; private set; }

        public IEnumerable<IItemEventSubscriber> ItemEventSubscribers { get; set; }

        public DefenseEquipmentFactory DefenseEquipmentFactory { get; set; }
        public WeaponFactory WeaponFactory { get; set; }
        public ContainerFactory ContainerFactory { get; set; }
        public RuneFactory RuneFactory { get; set; }
        public GroundFactory GroundFactory { get; set; }
        public CumulativeFactory CumulativeFactory { get; set; }
        public GenericItemFactory GenericItemFactory { get; set; }
        public IItemTypeStore ItemTypeStore { get; set; }
        public ICoinTypeStore CoinTypeStore { get; set; }

        public event CreateItem OnItemCreated;


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
            var coinsToAdd = CoinCalculator.Calculate(CoinTypeStore.Map, amount);

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

            return item is null ? null : Create(item.TypeId, location, attributes);
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
            itemType.Attributes.SetAttribute(attributes);
            
            if (itemType.TypeId < 100) return null;

            if (itemType.Group == ItemGroup.ItemGroupDeprecated) return null;

            if (itemType.Attributes.GetAttribute(ItemAttribute.Script) is { } script &&
                !string.IsNullOrWhiteSpace(script))
            {
                var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .FirstOrDefault(x => x.Name.Equals(script) || (x.FullName?.Equals(script) ?? false));

                if (type is not null &&
                    Activator.CreateInstance(type, itemType, location, attributes) is IItem instance) return instance;
            }

            if (DefenseEquipmentFactory.Create(itemType, location) is { } equipment) return equipment;
            if (WeaponFactory.Create(itemType, location, attributes) is { } weapon) return weapon;
            if (ContainerFactory.Create(itemType, location) is { } container) return container;
            if (RuneFactory.Create(itemType, location, attributes) is { } rune) return rune;
            if (GroundFactory.Create(itemType, location) is { } ground) return ground;

            if (CumulativeFactory.Create(itemType, location, attributes) is { } cumulative) return cumulative;

            if (LiquidPool.IsApplicable(itemType)) return new LiquidPool(itemType, location, attributes);
            if (MagicField.IsApplicable(itemType)) return new MagicField(itemType, location);
            if (FloorChanger.IsApplicable(itemType)) return new FloorChanger(itemType, location);
            if (TeleportItem.IsApplicable(itemType)) return new TeleportItem(itemType, location, attributes);
            
            if (UsableOnItem.IsApplicable(itemType))
            {
                if (FloorChangerUsableItem.IsApplicable(itemType))
                    return new FloorChangerUsableItem(itemType, location);
                if (TransformerUsableItem.IsApplicable(itemType)) return new TransformerUsableItem(itemType, location);
            }


            return GenericItemFactory.Create(itemType, location);
        }
    }
}