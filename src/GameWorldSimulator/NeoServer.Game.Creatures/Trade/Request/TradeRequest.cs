using System;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Creatures.Trade.Request;

public class TradeRequest
{
    public TradeRequest(IPlayer playerRequesting, IPlayer playerRequested, IItem item)
    {
        PlayerRequesting = (Player.Player)playerRequesting;
        PlayerRequested = (Player.Player)playerRequested;
        Item = item;
    }

    public Player.Player PlayerRequesting { get;  }
    public Player.Player PlayerRequested { get; }
    public IItem Item { get; }

    public bool PlayerAcknowledgedTrade => PlayerRequested.LastTradeRequest.Item is {};
}