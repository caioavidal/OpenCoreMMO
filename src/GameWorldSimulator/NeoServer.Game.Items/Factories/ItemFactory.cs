using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Items.Containers;
using NeoServer.Game.Items.Items.UsableItems;
using CreateItem = NeoServer.Game.Common.Contracts.Items.CreateItem;

namespace NeoServer.Game.Items.Factories;

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
    public IActionIdMapStore ActionIdMapStore { get; set; }
    public ICoinTypeStore CoinTypeStore { get; set; }
    public IActionStore ActionStore { get; set; }
    public IQuestStore QuestStore { get; set; }

    public IMap Map { get; set; }
    public event CreateItem OnItemCreated;

    public IItem CreateLootCorpse(ushort typeId, Location location, ILoot loot)
    {
        if (!ItemTypeStore.TryGetValue(typeId, out var itemType)) return null;

        var createdItem = new LootContainer(itemType, location, loot);

        SubscribeEvents(createdItem);

        OnItemCreated?.Invoke(createdItem);

        AddToActionIdMapStore(createdItem);

        return createdItem;
    }

    public IItem Create(ushort typeId, Location location, IDictionary<ItemAttribute, IConvertible> attributes,
        IEnumerable<IItem> children = null)
    {
        if (!ItemTypeStore.TryGetValue(typeId, out var itemType)) return null;

        var createdItem = CreateItem(itemType, location, attributes, children);

        SetItemIds(attributes, createdItem);

        SubscribeEvents(createdItem);

        OnItemCreated?.Invoke(createdItem);

        AddToActionIdMapStore(createdItem);

        return createdItem;
    }

    public IItem Create(IItemType itemType, Location location, IDictionary<ItemAttribute, IConvertible> attributes,
        IEnumerable<IItem> children = null)
    {
        var createdItem = CreateItem(itemType, location, attributes, children);

        SetItemIds(attributes, createdItem);

        SubscribeEvents(createdItem);

        OnItemCreated?.Invoke(createdItem);

        AddToActionIdMapStore(createdItem);
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

            OnItemCreated?.Invoke(newCoin);

            AddToActionIdMapStore(newCoin);

            yield return newCoin;
        }
    }

    public IItem Create(string name, Location location, IDictionary<ItemAttribute, IConvertible> attributes,
        IEnumerable<IItem> children = null)
    {
        var item = ItemTypeStore.All.FirstOrDefault(x =>
            x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        return item is null ? null : Create(item.TypeId, location, attributes, children);
    }

    private static void SetItemIds(IDictionary<ItemAttribute, IConvertible> attributes, IItem createdItem)
    {
        if (Guard.AnyNull(attributes, createdItem)) return;
        if (!attributes.Any()) return;

        attributes.TryGetValue(ItemAttribute.ActionId, out var actionId);
        attributes.TryGetValue(ItemAttribute.UniqueId, out var uniqueId);

        if (actionId is not null) createdItem.SetActionId((ushort)actionId);
        if (uniqueId is not null) createdItem.SetUniqueId(Convert.ToUInt32(uniqueId));
    }

    public event CreateItem OnItemWithActionIdCreated;

    private void SubscribeEvents(IItem createdItem)
    {
        if (Guard.IsNull(createdItem)) return;

        if (ItemEventSubscribers is null) return;

        foreach (var gameSubscriber in ItemEventSubscribers.Where(x =>
                     x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //register game events first
            gameSubscriber.Subscribe(createdItem);

        foreach (var subscriber in ItemEventSubscribers.Where(x =>
                     !x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //than register server events
            subscriber.Subscribe(createdItem);
    }

    private void AddToActionIdMapStore(IItem item)
    {
        if (Guard.IsNull(item)) return;

        item.Metadata.Attributes.TryGetAttribute<ushort>(ItemAttribute.ActionId, out var actionId);
        if (actionId == default) return;

        if (ActionIdMapStore.TryGetValue(actionId, out var items))
        {
            items.Add(item);
            OnItemWithActionIdCreated?.Invoke(item);
            return;
        }

        ActionIdMapStore?.Add(actionId, new List<IItem> { item });
        OnItemWithActionIdCreated?.Invoke(item);
    }

    private IItem CreateItem(IItemType itemType, Location location,
        IDictionary<ItemAttribute, IConvertible> attributes, IEnumerable<IItem> children)
    {
        itemType.Attributes.SetAttribute(attributes);

        if (itemType.TypeId < 100) return null;

        if (itemType.Group == ItemGroup.ItemGroupDeprecated) return null;

        if (TryCreateItemFromActionScript(itemType, location, attributes, out var createdItem)) return createdItem;

        if (TryCreateItemFromQuestScript(itemType, location, attributes, out var createdQuest)) return createdQuest;

        if (itemType.Attributes.GetAttribute(ItemAttribute.Script) is { } script)
            if (CreateItemFromScript(itemType, location, attributes, script) is { } instance)
                return instance;

        if (DefenseEquipmentFactory?.Create(itemType, location) is { } equipment) return equipment;
        if (WeaponFactory?.Create(itemType, location, attributes) is { } weapon) return weapon;
        if (ContainerFactory?.Create(itemType, location, children) is { } container) return container;
        if (RuneFactory?.Create(itemType, location, attributes) is { } rune) return rune;
        if (GroundFactory?.Create(itemType, location) is { } ground) return ground;

        if (CumulativeFactory?.Create(itemType, location, attributes) is { } cumulative) return cumulative;

        if (LiquidPool.IsApplicable(itemType)) return new LiquidPool(itemType, location, attributes);
        if (MagicField.IsApplicable(itemType)) return new MagicField(itemType, location);
        if (FloorChanger.IsApplicable(itemType)) return new FloorChanger(itemType, location);
        if (TeleportItem.IsApplicable(itemType)) return new TeleportItem(itemType, location, attributes);
        if (Paper.IsApplicable(itemType)) return new Paper(itemType, location, attributes);
        if (Sign.IsApplicable(itemType)) return new Sign(itemType, location, attributes);

        if (UsableOnItem.IsApplicable(itemType))
        {
            if (FloorChangerUsableItem.IsApplicable(itemType))
                return new FloorChangerUsableItem(itemType, location);

            return new UsableOnItem(itemType, location);
        }


        return GenericItemFactory?.Create(itemType, location);
    }

    private static IItem CreateItemFromScript(IItemType itemType, Location location,
        IDictionary<ItemAttribute, IConvertible> attributes, string script)
    {
        if (string.IsNullOrWhiteSpace(script)) return null;

        script = script.Replace(".cs", string.Empty);

        var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .FirstOrDefault(x => x.Name.Equals(script) || (x.FullName?.Equals(script) ?? false));

        if (type is null) return null;

        return Activator.CreateInstance(type, itemType, location, attributes) as IItem;
    }

    private bool TryCreateItemFromActionScript(IItemType itemType, Location location,
        IDictionary<ItemAttribute, IConvertible> attributes,
        out IItem item)
    {
        item = null;
        if (!itemType.Attributes.TryGetAttribute<ushort>(ItemAttribute.ActionId, out var actionId)) return false;
        if (!ActionStore.TryGetValue(actionId, out var action)) return false;
        if (!action.Script.EndsWith(".cs", StringComparison.InvariantCultureIgnoreCase)) return false;

        var instance = CreateItemFromScript(itemType, location, attributes, action.Script);
        if (instance is null) return false;

        item = instance;
        return true;
    }

    private bool TryCreateItemFromQuestScript(IItemType itemType, Location location,
        IDictionary<ItemAttribute, IConvertible> attributes,
        out IItem item)
    {
        item = null;
        itemType.Attributes.TryGetAttribute<ushort>(ItemAttribute.ActionId, out var actionId);
        itemType.Attributes.TryGetAttribute<ushort>(ItemAttribute.UniqueId, out var uniqueId);

        if (actionId == 0 && uniqueId == 0) return false;

        if (!QuestStore.TryGetValue((actionId, uniqueId), out var quest)) return false;
        if (string.IsNullOrWhiteSpace(quest.Script)) return false;
        if (!quest.Script.EndsWith(".cs", StringComparison.InvariantCultureIgnoreCase)) return false;

        var instance = CreateItemFromScript(itemType, location, attributes, quest.Script);
        if (instance is null) return false;

        item = instance;
        return true;
    }
}