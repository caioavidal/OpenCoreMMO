using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Item.Items;
using NeoServer.Game.Item.Items.Containers;
using NeoServer.Game.Item.Items.UsableItems;
using CreateItem = NeoServer.Game.Common.Contracts.Items.CreateItem;

namespace NeoServer.Game.Item.Factories;

public class ItemFactory : IItemFactory
{
    public ICoinTypeStore CoinTypeStore;
    private readonly IEnumerable<IItemEventSubscriber> _itemEventSubscribers;
    public ContainerFactory ContainerFactory;
    public CumulativeFactory CumulativeFactory;
    public DefenseEquipmentFactory DefenseEquipmentFactory;
    public GenericItemFactory GenericItemFactory;
    public GroundFactory GroundFactory;
    public IItemTypeStore ItemTypeStore;
    public RuneFactory RuneFactory;
    public WeaponFactory WeaponFactory;

    public ItemFactory(
        DefenseEquipmentFactory defenseEquipmentFactory, 
        WeaponFactory weaponFactory,
        ContainerFactory containerFactory,
        RuneFactory runeFactory, 
        GroundFactory groundFactory, 
        CumulativeFactory cumulativeFactory,
        GenericItemFactory genericItemFactory, 
        IItemTypeStore itemTypeStore,
        ICoinTypeStore coinTypeStore)
    {
        DefenseEquipmentFactory = defenseEquipmentFactory;
        WeaponFactory = weaponFactory;
        ContainerFactory = containerFactory;
        RuneFactory = runeFactory;
        GroundFactory = groundFactory;
        CumulativeFactory = cumulativeFactory;
        GenericItemFactory = genericItemFactory;
        ItemTypeStore = itemTypeStore;
        CoinTypeStore = coinTypeStore;
        Instance = this;
    }

    public static IItemFactory Instance { get; private set; }
    public event CreateItem OnItemCreated;

    public IItem CreateLootCorpse(ushort typeId, Location location, ILoot loot)
    {
        if (!ItemTypeStore.TryGetValue(typeId, out var itemType)) return null;

        var createdItem = new LootContainer(itemType, location, loot);
        
        OnItemCreated?.Invoke(createdItem);

        return createdItem;
    }

    public IItem Create(ushort typeId, Location location, IDictionary<ItemAttribute, IConvertible> attributes,
        IEnumerable<IItem> children = null)
    {
        if (!ItemTypeStore.TryGetValue(typeId, out var itemType)) return null;

        var createdItem = CreateItem(itemType, location, attributes, children);
        
        SetItemIds(attributes, createdItem);

        OnItemCreated?.Invoke(createdItem);

        return createdItem;
    }

    public IItem Create(IItemType itemType, Location location, IDictionary<ItemAttribute, IConvertible> attributes,
        IEnumerable<IItem> children = null)
    {
        var createdItem = CreateItem(itemType, location, attributes, children);

        SetItemIds(attributes, createdItem);

        OnItemCreated?.Invoke(createdItem);

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

    private IItem CreateItem(IItemType itemType, Location location,
        IDictionary<ItemAttribute, IConvertible> attributes, IEnumerable<IItem> children)
    {
        itemType.Attributes.SetAttribute(attributes);

        if (itemType.TypeId < 100) return null;

        if (itemType.Group == ItemGroup.Deprecated) return null;

        if (itemType.Attributes.GetAttribute(ItemAttribute.Script) is { } script)
            if (ItemFromScriptFactory.Create(itemType, location, attributes, script) is { } instance)
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
}