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
    private static HashSet<uint> PlayerEventSubscription { get;  } = new();

    public static void Init(Action<TradeRequest> cancelTradeAction)
    {
        CancelTradeAction = cancelTradeAction;
    }

    public static void Subscribe(IPlayer player, IItem item)
    {
        if (player is { } && !PlayerEventSubscription.Contains(player.CreatureId))
        {
            player.OnCreatureMoved += OnPlayerMoved;
            player.OnLoggedOut += OnPlayerLogout;
            
            PlayerEventSubscription.Add(player.CreatureId);
        }

        if (item is not { }) return;

        item.OnDeleted += ItemDeleted;
        if (item is ICumulative cumulative) cumulative.OnReduced += ItemReduced;
    }

    public static void Unsubscribe(IPlayer player, IItem item)
    {
        if (player is { })
        {
            player.OnCreatureMoved -= OnPlayerMoved;
            player.OnLoggedOut -= OnPlayerLogout;
            
            PlayerEventSubscription.Remove(player.CreatureId);
        }

        if (item is not { }) return;

        item.OnDeleted -= ItemDeleted;
        if (item is ICumulative cumulative) cumulative.OnReduced -= ItemReduced;
    }

    private static void OnPlayerMoved(IWalkableCreature creature, Location fromLocation, Location toLocation,
        ICylinderSpectator[] spectators)
    {
        if (creature is not Player.Player player) return;

        if (creature.Location.IsNextTo(player.LastTradeRequest.PlayerRequested.Location)) return;

        CancelTradeAction?.Invoke(player.LastTradeRequest);
        //cancel trade
    }

    private static void OnPlayerLogout(IPlayer player)
    {
        CancelTradeAction?.Invoke(((Player.Player)player).LastTradeRequest);
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