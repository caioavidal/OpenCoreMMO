using System;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Creatures.Trade.Request;

namespace NeoServer.Game.Creatures.Trade;

public class TradeSystem : ITradeService
{
    private readonly ICreatureGameInstance _creatureGameInstance;

    public TradeSystem(ICreatureGameInstance creatureGameInstance)
    {
        _creatureGameInstance = creatureGameInstance;

        TradeRequestEventHandler.Init(creatureGameInstance, CancelTrade);
    }

    public void Request(IPlayer player, IPlayer secondPlayer, IItem item)
    {
        if (Guard.AnyNull(player, secondPlayer, item)) return;

        if (!TradeRequestValidation.IsValid(player, secondPlayer, item)) return;

        var tradeRequest = new TradeRequest(player, secondPlayer, item);

        ((Player.Player)player).LastTradeRequest = tradeRequest;
        ((Player.Player)secondPlayer).LastTradeRequest ??= new TradeRequest(null, player, null);

        TradeRequestEventHandler.Subscribe(player, secondPlayer, item);

        OnTradeRequest?.Invoke(tradeRequest);
    }

    public void CancelTrade(IPlayer player, TradeRequest tradeRequest)
    {
        TradeRequestEventHandler.Unsubscribe(tradeRequest.PlayerRequesting, tradeRequest.Item);
        TradeRequestEventHandler.Unsubscribe(tradeRequest.PlayerRequested,
            tradeRequest.PlayerRequested.LastTradeRequest.Item);

        tradeRequest.PlayerRequesting.LastTradeRequest = null;
        tradeRequest.PlayerRequested.LastTradeRequest = null;

        OnCancelled?.Invoke(tradeRequest);
    }

    #region Events

    public event CancelTrade OnCancelled;
    public event RequestTrade OnTradeRequest;

    #endregion
}

public delegate void CancelTrade(TradeRequest tradeRequest);

public delegate void RequestTrade(TradeRequest tradeRequest);