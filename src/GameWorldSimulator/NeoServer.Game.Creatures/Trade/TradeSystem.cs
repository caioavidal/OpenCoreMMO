using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Creatures.Trade.Request;

namespace NeoServer.Game.Creatures.Trade;

/// <summary>
///     Provides functionality for trading items between two players.
/// </summary>
public class TradeSystem : ITradeService
{
    private readonly TradeItemExchanger _tradeItemExchanger;

    public TradeSystem(TradeItemExchanger tradeItemExchanger)
    {
        _tradeItemExchanger = tradeItemExchanger;
        TradeRequestEventHandler.Init(Cancel);
    }

    /// <summary>
    ///     Requests a trade between two players.
    /// </summary>
    /// <param name="player">The player requesting the trade.</param>
    /// <param name="secondPlayer">The player being asked to trade.</param>
    /// <param name="item">The item being offered for trade.</param>
    public void Request(IPlayer player, IPlayer secondPlayer, IItem item)
    {
        if (Guard.AnyNull(player, secondPlayer, item)) return;

        if (!TradeRequestValidation.IsValid(player, secondPlayer, item)) return;

        // Create a new trade request object.
        var tradeRequest = new TradeRequest(player, secondPlayer, item);

        // Set the LastTradeRequest property for both players.
        ((Player.Player)player).CurrentTradeRequest = tradeRequest;
        ((Player.Player)secondPlayer).CurrentTradeRequest ??= new TradeRequest(null, player, null);

        ItemTradedTracker.TrackItem(item, tradeRequest);

        // Subscribe both players to the trade request event.
        TradeRequestEventHandler.Subscribe(player, item);
        TradeRequestEventHandler.Subscribe(secondPlayer, item);


        OnTradeRequest?.Invoke(tradeRequest);
    }

    /// <summary>
    /// Cancels and closes a trade request
    /// </summary>
    /// <param name="tradeRequest">The trade request to cancel.</param>
    public void Cancel(TradeRequest tradeRequest)
    {
        var playerRequested = tradeRequest.PlayerRequested;
        var playerRequesting = tradeRequest.PlayerRequesting;
        
        Close(tradeRequest);

        const string message = "Trade is cancelled.";
        OperationFailService.Send(playerRequested.CreatureId, message);
        OperationFailService.Send(playerRequesting.CreatureId, message);
    }

    /// <summary>
    ///     Closes a trade request and cleans up any event subscriptions.
    /// </summary>
    /// <param name="tradeRequest">The trade request to close.</param>
    private void Close(TradeRequest tradeRequest)
    {
        if (Guard.IsNull(tradeRequest)) return;

        // Unsubscribe both players from the trade request event.
        var itemFromPlayerRequesting = tradeRequest.Item;
        var itemFromPlayerRequested = tradeRequest.PlayerRequested.CurrentTradeRequest.Item;

        TradeRequestEventHandler.Unsubscribe(tradeRequest.PlayerRequesting, itemFromPlayerRequesting);
        TradeRequestEventHandler.Unsubscribe(tradeRequest.PlayerRequested, itemFromPlayerRequested);

        ItemTradedTracker.UntrackItem(itemFromPlayerRequesting);
        ItemTradedTracker.UntrackItem(itemFromPlayerRequested);

        // Reset the LastTradeRequest property for both players.
        tradeRequest.PlayerRequesting.CurrentTradeRequest = null;
        tradeRequest.PlayerRequested.CurrentTradeRequest = null;

        OnClosed?.Invoke(tradeRequest);
    }

    /// <summary>
    ///     Accepts a trade request and initiates the item exchange.
    /// </summary>
    /// <param name="player">The player accepting the trade request.</param>
    public void AcceptTrade(IPlayer player)
    {
        var tradeRequest = ((Player.Player)player).CurrentTradeRequest;

        if (tradeRequest is null) return;

        tradeRequest.Accept();

        var playerRequested = tradeRequest.PlayerRequested;
        var lastTradeRequest = playerRequested.CurrentTradeRequest;

        if (lastTradeRequest is null || !lastTradeRequest.Accepted) return;

        var result = _tradeItemExchanger.Exchange(tradeRequest);

        // Close the trade request.
        Close(tradeRequest);

        if (result) OnTradeAccepted?.Invoke(tradeRequest);
    }

    #region Events

    public event CloseTrade OnClosed;
    public event RequestTrade OnTradeRequest;
    public event TradeAccept OnTradeAccepted;

    #endregion
}

public delegate void CloseTrade(TradeRequest tradeRequest);
public delegate void RequestTrade(TradeRequest tradeRequest);
public delegate void TradeAccept(TradeRequest tradeRequest);