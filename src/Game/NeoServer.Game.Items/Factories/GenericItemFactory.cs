using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Bases;
using NeoServer.Game.Items.Factories.AttributeFactory;

namespace NeoServer.Game.Items.Factories;

public class GenericItemFactory : IFactory
{
    public event CreateItem OnItemCreated;

    public IItem Create(IItemType itemType, Location location)
    {
        var hasDecayable = DecayableFactory.HasDecayable(itemType);

        if (!hasDecayable)
        {
            if (itemType.HasFlag(ItemFlag.Pickupable)) return new PickupableItem(itemType, location);
            if (itemType.HasFlag(ItemFlag.Moveable)) return new MovableItem(itemType, location);
            return new StaticItem(itemType, location);
        }

        return new Item(itemType, location);
    }
}