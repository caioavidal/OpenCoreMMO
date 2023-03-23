using System;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Creatures.Trade.Request;

internal static class TradeRequestEventHandler
{
    private static ICreatureGameInstance _creatureGameInstance;
    private static Action<TradeRequest> _cancelTradeAction;

    public static void Init(ICreatureGameInstance creatureGameInstance, Action<TradeRequest> cancelTradeAction)
    {
        _creatureGameInstance ??= creatureGameInstance;
        _cancelTradeAction = cancelTradeAction;
    }

    public static void Subscribe(IPlayer firstPlayer, IPlayer secondPlayer, IItem itemFromFirstPlayer)
    {
        firstPlayer.OnCreatureMoved += OnPlayerMoved;
        secondPlayer.OnCreatureMoved += OnPlayerMoved;

        firstPlayer.OnLoggedOut += OnPlayerLogout;
        secondPlayer.OnLoggedOut += OnPlayerLogout;

        itemFromFirstPlayer.OnDeleted += ItemDeleted;
        if (itemFromFirstPlayer is ICumulative cumulative) cumulative.OnReduced += ItemReduced;
    }

    public static void Subscribe(IItem itemFromSecondPlayer)
    {
        itemFromSecondPlayer.OnDeleted += ItemDeleted;
        if (itemFromSecondPlayer is ICumulative cumulative) cumulative.OnReduced += ItemReduced;
    }

    public static void Unsubscribe(IPlayer firstPlayer, IPlayer secondPlayer, IItem itemFromFirstPlayer,
        IItem itemFromSecondPlayer = null)
    {
        firstPlayer.OnCreatureMoved -= OnPlayerMoved;
        secondPlayer.OnCreatureMoved -= OnPlayerMoved;

        firstPlayer.OnLoggedOut -= OnPlayerLogout;
        secondPlayer.OnLoggedOut -= OnPlayerLogout;

        itemFromFirstPlayer.OnDeleted -= ItemDeleted;
        if (itemFromFirstPlayer is ICumulative cumulative) cumulative.OnReduced -= ItemReduced;

        if (itemFromSecondPlayer is null) return;

        itemFromSecondPlayer.OnDeleted -= ItemDeleted;
        if (itemFromSecondPlayer is ICumulative cumulative2) cumulative2.OnReduced -= ItemReduced;
    }

    private static void OnPlayerMoved(IWalkableCreature creature, Location fromLocation, Location toLocation,
        ICylinderSpectator[] spectators)
    {
        if (creature is not Player.Player player) return;

        var isFirstPlayer = player.CreatureId == player.LastTradeRequest.FirstPlayer;

        var idOfPlayerTradingWith =
            isFirstPlayer ? player.LastTradeRequest.SecondPlayer : player.LastTradeRequest.FirstPlayer;

        _creatureGameInstance.TryGetCreature(idOfPlayerTradingWith, out var playerTradingWith);

        if (playerTradingWith is null)
        {
            _cancelTradeAction?.Invoke(player.LastTradeRequest);
            return;
        }

        if (creature.Location.IsNextTo(playerTradingWith.Location)) return;

        _cancelTradeAction?.Invoke(player.LastTradeRequest);
        //cancel trade
    }

    private static void OnPlayerLogout(IPlayer player)
    {
        _cancelTradeAction?.Invoke(((Player.Player)player).LastTradeRequest);
        //cancel trade
    }

    public static void OnItemMoved()
    {
    }

    private static void ItemDeleted(IItem item)
    {
        //cancel trade
    }

    private static void ItemReduced(ICumulative item, byte amount)
    {
        //cancel trade
    }
}