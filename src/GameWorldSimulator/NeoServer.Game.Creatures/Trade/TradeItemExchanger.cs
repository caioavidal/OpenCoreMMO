using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Creatures.Trade.Request;

namespace NeoServer.Game.Creatures.Trade;

/// <summary>
/// A class that allows players to exchange items with each other in a game.
/// </summary>
/// <remarks>
/// The class takes in an IItemRemoveService object in its constructor to remove items from the world when they are exchanged.
/// It contains methods to check if a trade can be performed, add items to a player's inventory, and exchange items between players.
/// </remarks>
public class TradeItemExchanger
{
    private readonly IItemRemoveService _itemRemoveService;

    public TradeItemExchanger(IItemRemoveService itemRemoveService)
    {
        _itemRemoveService = itemRemoveService;
    }

    private const string DEFAULT_ERROR_MESSAGE = "Trade could not be completed.";

    /// <summary>
    /// Attempts to exchange items between two players.
    /// </summary>
    /// <param name="tradeRequest">The trade request containing the two players and their items to exchange.</param>
    /// <returns>True if the exchange was successful, false otherwise.</returns>
    public bool Exchange(TradeRequest tradeRequest)
    {
        var playerRequesting = tradeRequest.PlayerRequesting;
        var playerRequested = tradeRequest.PlayerRequested;

        // Get the last item requested from each player
        var itemFromPlayerRequesting = tradeRequest.PlayerRequesting.LastTradeRequest.Item;
        var itemFromPlayerRequested = tradeRequest.PlayerRequested.LastTradeRequest.Item;

        if (!CanPerformTrade(playerRequested, itemFromPlayerRequesting, playerRequesting, itemFromPlayerRequested))
            return false;

        ExchangeItem(playerRequesting, playerRequested, itemFromPlayerRequested, itemFromPlayerRequesting);

        return true;
    }

    private void ExchangeItem(IPlayer playerRequesting, IPlayer playerRequested, IItem itemFromPlayerRequested,
        IItem itemFromPlayerRequesting)
    {
        if (Guard.AnyNull(itemFromPlayerRequested, playerRequesting, itemFromPlayerRequesting, playerRequested)) return;

        // Remove the items from their previous locations in the world
        _itemRemoveService.RemoveFromWorld(itemFromPlayerRequesting);
        _itemRemoveService.RemoveFromWorld(itemFromPlayerRequested);

        // Add the items to each player's inventory
        AddItemToInventory(playerRequesting, itemFromPlayerRequested);
        AddItemToInventory(playerRequested, itemFromPlayerRequesting);
    }

    private static bool CanPerformTrade(IPlayer playerRequested, IItem itemFromPlayerRequesting,
        IPlayer playerRequesting,
        IItem itemFromPlayerRequested)
    {
        // Check if playerRequested has enough inventory space to add itemFromPlayerRequesting
        if (!CanAddItem(playerRequested, itemFromPlayerRequesting))
        {
            OperationFailService.Send(playerRequesting.CreatureId, DEFAULT_ERROR_MESSAGE);
            return false;
        }

        // Check if playerRequesting has enough inventory space to add itemFromPlayerRequested
        if (CanAddItem(playerRequesting, itemFromPlayerRequested)) return true;

        // Send an error message to both players if the trade cannot be performed
        OperationFailService.Send(playerRequested.CreatureId, DEFAULT_ERROR_MESSAGE);
        OperationFailService.Send(playerRequesting.CreatureId, DEFAULT_ERROR_MESSAGE);

        return false;
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
                OperationFailService.Send(player.CreatureId,
                    $"You do not have enough capacity to carry this object.\nIt weighs {item.Weight} oz.");
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
        if (item.Metadata.BodyPosition is Slot.None) return Slot.Backpack;
        return player.Inventory[item.Metadata.BodyPosition] is null ? item.Metadata.BodyPosition : Slot.Backpack;
    }
}