using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Item.Factories;
using NeoServer.Game.Item.Factories.AttributeFactory;

namespace NeoServer.Game.Tests.Server;

public static class ItemFactoryTestBuilder
{
    public static IItemFactory Build(params IItemType[] itemTypes)
    {
        var itemTypeStore = ItemTypeStoreTestBuilder.Build(itemTypes);

        return new ItemFactory(null, weaponFactory: new WeaponFactory(new ChargeableFactory(), itemTypeStore),
            null, null, null, null, null, itemTypeStore, null);
    }
    
    public static IItemFactory Build(IItemTypeStore itemTypeStore, IMap map = null)
    {
        var chargeableFactory = new ChargeableFactory();

        return new ItemFactory(new DefenseEquipmentFactory(itemTypeStore, chargeableFactory),
            weaponFactory: new WeaponFactory(chargeableFactory, itemTypeStore),
            null, null, null, null, null, itemTypeStore, null);
    }
}


