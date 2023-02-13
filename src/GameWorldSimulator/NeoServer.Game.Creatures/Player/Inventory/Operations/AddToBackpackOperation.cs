using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Creatures.Player.Inventory.Operations;

public static class AddToBackpackOperation
{
    public static Result<IPickupable> Add(Inventory inventory, IPickupable item)
    {
        if (inventory.InventoryMap.GetItem<IPickupableContainer>(Slot.Backpack) is { } backpack)
            return new Result<IPickupable>(null, backpack.AddItem(item).Error);

        if (item is IPickupableContainer container) container.SetParent(inventory.Owner);

        inventory.InventoryMap.Add(Slot.Backpack, item, item.ClientId);

        ((IMovableThing)item).SetNewLocation(Location.Inventory(Slot.Backpack));

        //      OnItemAddedToSlot?.Invoke(this, item, slot);
        return Result<IPickupable>.Success;
    }
}