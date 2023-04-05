using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Services;

namespace NeoServer.Game.Creatures.Trade.Validations;

internal static class TradeExchangeValidation
{
    private const string DEFAULT_ERROR_MESSAGE = "Trade could not be completed.";

    public static bool CanPerformTrade(IPlayer playerRequested, IItem itemFromPlayerRequesting,
        IPlayer playerRequesting,
        IItem itemFromPlayerRequested)
    {
        // Check if playerRequested has enough inventory space to add itemFromPlayerRequesting
        if (!CanAddItem(playerRequested, itemFromPlayerRequesting, itemFromPlayerRequested))
        {
            OperationFailService.Send(playerRequesting.CreatureId, DEFAULT_ERROR_MESSAGE);
            return false;
        }

        // Check if playerRequesting has enough inventory space to add itemFromPlayerRequested
        if (CanAddItem(playerRequesting, itemFromPlayerRequested, itemFromPlayerRequesting)) return true;

        // Send an error message to both players if the trade cannot be performed
        OperationFailService.Send(playerRequested.CreatureId, DEFAULT_ERROR_MESSAGE);
        OperationFailService.Send(playerRequesting.CreatureId, DEFAULT_ERROR_MESSAGE);

        return false;
    }
    
    private static bool CanAddItem(IPlayer player, IItem item, IItem itemToBeRemoved)
    {
        var inventory = player.Inventory;

        var slotDestination = GetSlotDestination(player, item);
        
        if (itemToBeRemoved == inventory[Slot.Backpack])
        {
            //player is trading his own backpack and slot destination is backpack then cancel the trade
            if (item.Metadata.BodyPosition is Slot.None) return false;
        }

        if (!PlayerHasEnoughCapacity(player, item, itemToBeRemoved)) return false;

        if (itemToBeRemoved == inventory[slotDestination])
        {
            //player has space to allocate the new item
            return true;
        }

        if (!PlayerHasFreeSlotsToAddTheItem(player, item, slotDestination)) return false;

        return true;
    }

    private static bool PlayerHasFreeSlotsToAddTheItem(IPlayer player, IItem item, Slot slotDestination)
    {
        var possibleAmountToAdd = player.Inventory.PossibleAmountToAdd(item, (byte)slotDestination);

        if (possibleAmountToAdd < item.Amount)
        {
            OperationFailService.Send(player.CreatureId, "You do not have enough room to carry this object.");
            return false;
        }

        return true;
    }

    private static bool PlayerHasEnoughCapacity(IPlayer player, IItem item, IItem itemToBeRemoved)
    {
        var capacityAfterItemRemoval = player.TotalCapacity - player.Inventory.TotalWeight + itemToBeRemoved.Weight;

        if (item.Weight > capacityAfterItemRemoval)
        {
            OperationFailService.Send(player.CreatureId,
                $"You do not have enough capacity to carry this object.\nIt weighs {item.Weight} oz.");
            return false;
        }

        return true;
    }

    private static Slot GetSlotDestination(IPlayer player, IItem item)
    {
        if (item.Metadata.BodyPosition is Slot.None) return Slot.Backpack;
        return player.Inventory[item.Metadata.BodyPosition] is null ? item.Metadata.BodyPosition : Slot.Backpack;
    }
}