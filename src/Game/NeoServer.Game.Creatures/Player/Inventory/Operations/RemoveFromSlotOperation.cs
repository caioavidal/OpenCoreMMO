using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Game.Creatures.Player.Inventory.Operations;

internal static class RemoveFromSlotOperation
{
    internal static Result<IPickupable> Remove(Inventory inventory, Slot slot, byte amount)
    {
        if (amount == 0) return Result<IPickupable>.Fail(InvalidOperation.Impossible);
        
        if (inventory.InventoryMap.GetItem<IPickupable>(slot) is not { } item) return Result<IPickupable>.Fail(InvalidOperation.Impossible);

        var removedItem = GetRemovedItem(inventory, slot, amount, item);

        if (removedItem is IDressable dressable) dressable.UndressFrom(inventory.Owner);
        
        if(removedItem is ICumulative cumulative) cumulative.ClearSubscribers(); 

        return Result<IPickupable>.Ok(removedItem);
    }

    private static IPickupable GetRemovedItem(Inventory inventory, Slot slot, byte amount, IPickupable item)
    {
        if (item is ICumulative cumulative && amount < cumulative.Amount)
        {
            return cumulative.Split(amount);
        }

        if (item is ICumulative c) c.ClearSubscribers();

        inventory.InventoryMap.Remove(slot);
        return item;
    }
}