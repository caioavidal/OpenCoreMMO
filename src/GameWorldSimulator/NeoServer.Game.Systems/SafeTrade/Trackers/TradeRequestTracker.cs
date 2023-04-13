using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Systems.SafeTrade.Request;

namespace NeoServer.Game.Systems.SafeTrade.Trackers;

internal static class TradeRequestTracker
{
    private static readonly Dictionary<IPlayer, TradeRequest> TradeRequests = new();

    public static void Track(TradeRequest tradeRequest)
    {
        TradeRequests[tradeRequest.PlayerRequesting] = tradeRequest;
    }

    public static bool Untrack(IPlayer player)
    {
        return player is not null && TradeRequests.Remove(player);
    }

    public static TradeRequest GetTradeRequest(IPlayer player)
    {
        return TradeRequests.TryGetValue(player, out var tradeRequest) ? tradeRequest : null;
    }
}