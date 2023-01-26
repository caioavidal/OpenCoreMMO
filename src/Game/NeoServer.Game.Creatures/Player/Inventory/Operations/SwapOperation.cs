using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Game.Creatures.Player.Inventory.Operations;

public static class SwapOperation
{
    public static Result<IPickupable> SwapItem(Inventory inventory, Slot slot, IPickupable newItem)
    {
        var (existingItem, _) = inventory.InventoryMap.GetItem(slot);

        if (TryJoinCumulativeItem(newItem, existingItem, out var swappedItem))
            return Result<IPickupable>.Ok(swappedItem);

        var amountToRemove = (byte)(newItem.Amount + existingItem.Amount > 100
            ? newItem.Amount + existingItem.Amount - 100
            : existingItem.Amount);

        var result = RemoveFromSlotOperation.Remove(inventory, slot, amountToRemove);

        if (result.Failed) return result;

        var removedItem = result.Value;

        var addResult = AddToSlotOperation.Add(inventory, slot, newItem);

        return addResult.Failed ? result : Result<IPickupable>.Ok(removedItem);
    }

    private static bool TryJoinCumulativeItem(IItem newItem, IItem existingItem,
        out ICumulative swappedItem)
    {
        swappedItem = null;

        if (existingItem.ClientId != newItem.ClientId) return false;
        if (existingItem is not ICumulative existingCumulativeItem) return false;
        if (newItem is not ICumulative newCumulativeItem) return false;

        if (newItem.Amount + existingItem.Amount <= 100) return false;

        existingCumulativeItem.TryJoin(ref newCumulativeItem);

        if (newCumulativeItem?.Amount > 0) swappedItem = newCumulativeItem;

        return true;
    }
}