using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Game.Creatures.Player.Inventory.Rules;

public abstract class SwapRule
{
    public static bool ShouldSwap(Inventory inventory, IPickupable itemToAdd, Slot slotDestination)
    {
        if (slotDestination == Slot.Backpack) return false;

        if (inventory.InventoryMap.GetItem<IPickupable>(slotDestination) is not { } itemOnSlot) return false;

        if (itemToAdd is ICumulative cumulative && itemOnSlot.ClientId == cumulative.ClientId &&
            itemOnSlot.Amount + itemToAdd.Amount <= 100)
            //will join
            return false;

        return true;
    }
}