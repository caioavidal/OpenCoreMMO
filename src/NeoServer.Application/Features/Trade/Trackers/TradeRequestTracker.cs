using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Application.Features.Trade.Trackers;

internal static class TradeRequestTracker
{
    private static readonly Dictionary<IPlayer, Request.TradeRequest> TradeRequests = new();

    public static void Track(Request.TradeRequest tradeRequest)
    {
        TradeRequests[tradeRequest.PlayerRequesting] = tradeRequest;
    }

    public static bool Untrack(IPlayer player)
    {
        return player is not null && TradeRequests.Remove(player);
    }

    public static Request.TradeRequest GetTradeRequest(IPlayer player)
    {
        return TradeRequests.TryGetValue(player, out var tradeRequest) ? tradeRequest : null;
    }
}