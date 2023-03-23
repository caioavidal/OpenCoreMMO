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

        var tradeRequest = new TradeRequest
        {
            FirstPlayer = player.CreatureId,
            SecondPlayer = secondPlayer.CreatureId,
            ItemFromFirstPlayer = item,
            RequestedAt = DateTime.Now
        };
        
        ((Player.Player)player).LastTradeRequest = tradeRequest;
        ((Player.Player)secondPlayer).LastTradeRequest = tradeRequest;
        
        TradeRequestEventHandler.Subscribe(player, secondPlayer, item);

        OnTradeRequest?.Invoke(tradeRequest);
    }

    public void CancelTrade(TradeRequest tradeRequest)
    {
        _creatureGameInstance.TryGetCreature(tradeRequest.FirstPlayer, out var playerOne);
        _creatureGameInstance.TryGetCreature(tradeRequest.FirstPlayer, out var playerTwo);

        if (playerOne is not Player.Player firstPlayer || playerTwo is not Player.Player secondPlayer)
        {
            OnCancelled?.Invoke(tradeRequest);
            return;
        }
        
        TradeRequestEventHandler.Unsubscribe(firstPlayer, secondPlayer, firstPlayer.LastTradeRequest?.ItemFromFirstPlayer,
            firstPlayer.LastTradeRequest?.ItemFromSecondPlayer);
        
        firstPlayer.LastTradeRequest = null;
        secondPlayer.LastTradeRequest = null;
        
        OnCancelled?.Invoke(tradeRequest);
    }

    #region Events

    public event CancelTrade OnCancelled;
    public event RequestTrade OnTradeRequest;

    #endregion
}

public delegate void CancelTrade(TradeRequest tradeRequest);
public delegate  void RequestTrade(TradeRequest tradeRequest);