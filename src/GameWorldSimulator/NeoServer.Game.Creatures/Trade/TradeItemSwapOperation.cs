using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Creatures.Trade.Request;

namespace NeoServer.Game.Creatures.Trade;

public static class TradeItemSwapOperation
{
    private const string DEFAULT_ERROR_MESSAGE = "Trade could not be completed.";

    public static bool Swap(TradeRequest tradeRequest)
    {
        var playerRequesting = tradeRequest.PlayerRequesting;
        var playerRequested = tradeRequest.PlayerRequested;

        var itemFromPlayerRequesting = tradeRequest.PlayerRequesting.LastTradeRequest.Item;
        var itemFromPlayerRequested = tradeRequest.PlayerRequested.LastTradeRequest.Item;

        if (!CanAddItem(playerRequested, itemFromPlayerRequesting))
        {
            OperationFailService.Send(playerRequesting.CreatureId, DEFAULT_ERROR_MESSAGE);
            return false;
        }

        if (!CanAddItem(playerRequesting, itemFromPlayerRequested))
        {
            OperationFailService.Send(playerRequested.CreatureId, DEFAULT_ERROR_MESSAGE);
            return false;
        }

        AddItemToInventory(playerRequested, itemFromPlayerRequesting);
        AddItemToInventory(playerRequesting, itemFromPlayerRequested);

        return true;
    }

    private static bool CanAddItem(IPlayer player, IItem item)
    {
        var slot = GetSlotDestination(player, item);
        var result = player.Inventory.CanAddItem(item, item.Amount, (byte)slot);

        if (result.Succeeded)
        {
            return true;
        }

        switch (result.Error)
        {
            case InvalidOperation.NotEnoughRoom:
                OperationFailService.Send(player.CreatureId, "You do not have enough room to carry this object.");
                break;
            case InvalidOperation.TooHeavy:
                OperationFailService.Send(player.CreatureId, $"You do not have enough capacity to carry this object.\nIt weighs {item.Weight} oz.");
                break;
            default:
                OperationFailService.Send(player.CreatureId, DEFAULT_ERROR_MESSAGE);
                break;
        }

        return false;
    }

    private static void AddItemToInventory(IPlayer player, IItem item)
    {
        var slot = GetSlotDestination(player, item);
        player.Inventory.AddItem(item, slot);
    }

    private static Slot GetSlotDestination(IPlayer player, IItem item)
    {
        return player.Inventory[item.Metadata.BodyPosition] != null ? item.Metadata.BodyPosition : Slot.Backpack;
    }
}