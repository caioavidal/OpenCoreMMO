using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.Containers;

public class PickupableContainer : Container.Container, IPickupableContainer
{
    public PickupableContainer(IItemType type, Location location, List<IItem> children) : base(type, location,
        children)
    {
    }
    public new static bool IsApplicable(IItemType type)
    {
        return (type.Group == ItemGroup.GroundContainer ||
                type.Attributes.GetAttribute(ItemAttribute.Type)?.ToLower() == "container") &&
               type.HasFlag(ItemFlag.Pickupable);
    }
}