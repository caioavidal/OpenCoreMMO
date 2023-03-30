using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Creatures.Trade.Request;

internal static class TradeRequestEventHandler
{
    private static Action<TradeRequest> CancelTradeAction { get; set; }
    private static HashSet<uint> PlayerEventSubscription { get; } = new();

    /// <summary>
    ///     Initializes the TradeRequestEventHandler with the cancelTradeAction.
    /// </summary>
    public static void Init(Action<TradeRequest> cancelTradeAction)
    {
        CancelTradeAction = cancelTradeAction;
    }

    /// <summary>
    ///     Subscribes to the events of the given player and item, if they exist.
    /// </summary>
    public static void Subscribe(IPlayer player, IItem item)
    {
        if (player is { } && !PlayerEventSubscription.Contains(player.CreatureId))
        {
            player.OnCreatureMoved += OnPlayerMoved;
            player.OnLoggedOut += OnPlayerLogout;

            // Add player ID to the HashSet to prevent multiple subscriptions
            PlayerEventSubscription.Add(player.CreatureId);
        }

        if (item is not { }) return;

        item.OnDeleted += ItemDeleted;
        item.OnRemoved += ItemRemoved;
        if (item is ICumulative cumulative) cumulative.OnReduced += ItemReduced;
    }

    /// <summary>
    ///     Unsubscribes from the events of the given player and item, if they exist.
    /// </summary>
    public static void Unsubscribe(IPlayer player, IItem item)
    {
        if (player is { })
        {
            player.OnCreatureMoved -= OnPlayerMoved;
            player.OnLoggedOut -= OnPlayerLogout;

            // Remove player ID from the HashSet to allow future subscriptions
            PlayerEventSubscription.Remove(player.CreatureId);
        }

        if (item is not { }) return;

        item.OnDeleted -= ItemDeleted;
        item.OnRemoved -= ItemRemoved;
        if (item is ICumulative cumulative) cumulative.OnReduced -= ItemReduced;
    }

    private static void OnPlayerMoved(IWalkableCreature creature, Location fromLocation, Location toLocation,
        ICylinderSpectator[] spectators)
    {
        if (creature is not Player.Player player) return;

        // Check if the player moved to a location that is not next to the other player
        if (creature.Location.IsNextTo(player.CurrentTradeRequest.PlayerRequested.Location)) return;

        CancelTradeAction?.Invoke(player.CurrentTradeRequest);
    }

    private static void OnPlayerLogout(IPlayer player) =>
        CancelTradeAction?.Invoke(((Player.Player)player).CurrentTradeRequest);

    private static void ItemRemoved(IItem item, IThing _) => CancelTrade(item);
    private static void ItemDeleted(IItem item) => CancelTrade(item);
    private static void ItemReduced(ICumulative item, byte amount) => CancelTrade(item);

    private static void CancelTrade(IItem item)
    {
        var tradeRequest = ItemTradedTracker.GetTradeRequest(item);
        if (tradeRequest is null) return;

        CancelTradeAction?.Invoke(tradeRequest);
    }
}