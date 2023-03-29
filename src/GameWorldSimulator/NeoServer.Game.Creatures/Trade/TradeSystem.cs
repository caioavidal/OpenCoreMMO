using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Creatures.Trade.Request;

namespace NeoServer.Game.Creatures.Trade;

public class TradeSystem : ITradeService
{
    private readonly TradeItemSwapOperation _tradeItemSwapOperation;

    public TradeSystem(TradeItemSwapOperation tradeItemSwapOperation)
    {
        _tradeItemSwapOperation = tradeItemSwapOperation;
        TradeRequestEventHandler.Init(Close);
    }

    public void Request(IPlayer player, IPlayer secondPlayer, IItem item)
    {
        if (Guard.AnyNull(player, secondPlayer, item)) return;

        if (!TradeRequestValidation.IsValid(player, secondPlayer, item)) return;

        var tradeRequest = new TradeRequest(player, secondPlayer, item);

        ((Player.Player)player).LastTradeRequest = tradeRequest;
        ((Player.Player)secondPlayer).LastTradeRequest ??= new TradeRequest(null, player, null);

        TradeRequestEventHandler.Subscribe(player, item);
        TradeRequestEventHandler.Subscribe(secondPlayer, item);

        OnTradeRequest?.Invoke(tradeRequest);
    }

    public void Close(TradeRequest tradeRequest)
    {
        if (Guard.IsNull(tradeRequest)) return;
        
        TradeRequestEventHandler.Unsubscribe(tradeRequest.PlayerRequesting, tradeRequest.Item);
        TradeRequestEventHandler.Unsubscribe(tradeRequest.PlayerRequested,
            tradeRequest.PlayerRequested.LastTradeRequest.Item);

        tradeRequest.PlayerRequesting.LastTradeRequest = null;
        tradeRequest.PlayerRequested.LastTradeRequest = null;
        
        OnClosed?.Invoke(tradeRequest);
    }

    public void AcceptTrade(IPlayer player)
    {
        var tradeRequest = ((Player.Player)player).LastTradeRequest;

        if (tradeRequest is null) return;

        tradeRequest.Accept();

        if (!tradeRequest.PlayerRequested.LastTradeRequest.Accepted) return;

        var result = _tradeItemSwapOperation.Swap(tradeRequest);

        Close(tradeRequest);
        
        if (!result) return;

        OnTradeAccepted?.Invoke(tradeRequest);
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