using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Player.Inventory.Rules;

namespace NeoServer.Game.Creatures.Player.Inventory.Operations;

public abstract class AddToSlotOperation
{
    public static Result<IPickupable> Add(Inventory inventory, Slot slot, IPickupable item)
    {
        var result = inventory.CanAddItem(slot, item, item.Amount);

        if (result.Failed) return Result<IPickupable>.Fail(result.Error);
        
        if (SwapRule.ShouldSwap(inventory, item, slot))
        {
            return SwapOperation.SwapItem(inventory, slot, item);
        }
        
        if (slot is Slot.Backpack)
        {
            return AddToBackpackOperation.Add(inventory, item);
        }

        if (item is ICumulative cumulative)
        {
            var addCumulativeResult = AddCumulativeItemOperation.Add(inventory, cumulative, slot);
            
            if (result.Failed) return Result<IPickupable>.Fail(addCumulativeResult.Error);
        }

        inventory.InventoryMap.Add(slot, item, item.ClientId);

        if (item is IMovableThing movableThing) movableThing.SetNewLocation(Location.Inventory(slot));

        if (item is IDressable dressable) dressable.DressedIn(inventory.Owner);

      //  OnItemAddedToSlot?.Invoke(this, item, slot);
        return new Result<IPickupable>();
    }
}