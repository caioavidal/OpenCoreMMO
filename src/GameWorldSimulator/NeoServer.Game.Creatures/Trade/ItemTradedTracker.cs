using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Creatures.Trade.Request;

namespace NeoServer.Game.Creatures.Trade;

/// <summary>
/// A static class for tracking the trade requests associated with items.
/// </summary>
internal static class ItemTradedTracker
{
    private static readonly Dictionary<IItem, TradeRequest> ItemToTradeRequestMap = new();

    public static void TrackItem(IItem item, TradeRequest tradeRequest) => ItemToTradeRequestMap[item] = tradeRequest;

    public static bool UntrackItem(IItem item) => item is not null && ItemToTradeRequestMap.Remove(item);

    public static TradeRequest GetTradeRequest(IItem item) =>
        ItemToTradeRequestMap.TryGetValue(item, out var tradeRequest) ? tradeRequest : null;
}