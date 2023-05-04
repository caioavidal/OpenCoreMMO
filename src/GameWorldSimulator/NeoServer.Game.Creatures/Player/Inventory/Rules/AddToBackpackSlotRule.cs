using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Creatures.Player.Inventory.Rules;

public static class AddToBackpackSlotRule
{
    internal static Result CanAddToBackpackSlot(this Inventory inventory, IItem item)
    {
        if (item is IContainer container &&
            container.IsPickupable &&
            !inventory.InventoryMap.HasItemOnSlot(Slot.Backpack) &&
            item.Metadata.Attributes.GetAttribute(ItemAttribute.BodyPosition) == "backpack")
            return Result.Success;

        return inventory.InventoryMap.HasItemOnSlot(Slot.Backpack)
            ? Result.Success
            : Result.Fail(InvalidOperation.CannotDress);
    }
}