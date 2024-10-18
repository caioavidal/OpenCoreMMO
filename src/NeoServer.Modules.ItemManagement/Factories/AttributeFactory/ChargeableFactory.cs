using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Item.Items.Attributes;

namespace NeoServer.Modules.ItemManagement.Factories.AttributeFactory;

public class ChargeableFactory : IFactory
{
    public event CreateItem OnItemCreated;

    public IChargeable Create(IItemType itemType)
    {
        if (!itemType.Attributes.TryGetAttribute<ushort>(ItemAttribute.Charges, out var charges)) return null;
        if (!itemType.Attributes.TryGetAttribute<ushort>(ItemAttribute.ShowCharges, out var showCharges))
            return new Chargeable(charges, true);

        return new Chargeable(charges, showCharges == 1);
    }
}