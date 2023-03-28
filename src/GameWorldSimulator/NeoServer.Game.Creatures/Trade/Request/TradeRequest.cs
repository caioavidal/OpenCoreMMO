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
    public bool Accepted { get; private set; }
    public IItem Item { get; }

    public void Accept()
    {
        Accepted = true;
    }

    public bool PlayerAcknowledgedTrade => PlayerRequested.LastTradeRequest.Item is {};
}