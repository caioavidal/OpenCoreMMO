using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Systems.SafeTrade.Request;
using NeoServer.Game.Systems.SafeTrade.Validations;

namespace NeoServer.Game.Systems.SafeTrade.Operations;

/// <summary>
///     A class that allows players to exchange items with each other in a game.
/// </summary>
/// <remarks>
///     The class takes in an IItemRemoveService object in its constructor to remove items from the world when they are
///     exchanged.
///     It contains methods to check if a trade can be performed, add items to a player's inventory, and exchange items
///     between players.
/// </remarks>
public class TradeItemExchanger
{
    private const string DEFAULT_ERROR_MESSAGE = "Trade could not be completed.";
    private readonly IItemRemoveService _itemRemoveService;

    public TradeItemExchanger(IItemRemoveService itemRemoveService)
    {
        _itemRemoveService = itemRemoveService;
    }

    /// <summary>
    ///     Attempts to exchange items between two players.
    /// </summary>
    /// <param name="tradeRequest">The trade request containing the two players and their items to exchange.</param>
    /// <returns>True if the exchange was successful, false otherwise.</returns>
    public bool Exchange(TradeRequest tradeRequest)
    {
        // var playerRequesting = tradeRequest.PlayerRequesting;
        // var playerRequested = tradeRequest.PlayerRequested;
        //
        // // Get the last item requested from each player
        // var itemFromPlayerRequesting = tradeRequest.PlayerRequesting.CurrentTradeRequest.Items[0];
        // var itemFromPlayerRequested = tradeRequest.PlayerRequested.CurrentTradeRequest.Items[0];
        //
        // if (!TradeExchangeValidation.CanPerformTrade(playerRequested, itemFromPlayerRequesting, playerRequesting, itemFromPlayerRequested))
        //     return false;
        //
        // ExchangeItem(playerRequesting, playerRequested, itemFromPlayerRequested, itemFromPlayerRequesting);

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