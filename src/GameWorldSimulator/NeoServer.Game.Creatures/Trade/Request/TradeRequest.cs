using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Creatures.Trade.Request;

/// <summary>
/// Represents a trade request between two players in the game.
/// </summary>
public class TradeRequest
{
    public TradeRequest(IPlayer playerRequesting, IPlayer playerRequested, IItem item)
    {
        // Stores the players and item involved in the trade request
        PlayerRequesting = (Player.Player)playerRequesting;
        PlayerRequested = (Player.Player)playerRequested;
        Item = item;
    }
    
    /// <summary>
    /// The player initiating the trade.
    /// </summary>
    public Player.Player PlayerRequesting { get;  }
    
    /// <summary>
    /// The player being requested to trade.
    /// </summary>
    public Player.Player PlayerRequested { get; }
    
    /// <summary>
    /// Whether the trade request has been accepted.
    /// </summary>
    public bool Accepted { get; private set; }
    
    /// <summary>
    /// Whether the trade request has been accepted.
    /// </summary>
    public IItem Item { get; }

    /// <summary>
    /// Accepts the trade request.
    /// </summary>
    public void Accept()
    {
        Accepted = true;
    }

    /// <summary>
    /// Whether the player being requested to trade has acknowledged the trade request.
    /// </summary>
    public bool PlayerAcknowledgedTrade => PlayerRequested.LastTradeRequest.Item is {};
}