using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Creatures.Trade.Request;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Trade;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Server.Events.Player.Trade;

public class TradeRequestedEventHandler: IEventHandler
{
    private readonly IGameServer _gameServer;
    public TradeRequestedEventHandler(IGameServer gameServer)
    {
        _gameServer = gameServer;
    }
    public void Execute(TradeRequest tradeRequest)
    {
        if (Guard.AnyNull(tradeRequest, tradeRequest.PlayerRequesting, tradeRequest.PlayerRequested)) return;
        
        _gameServer.CreatureManager.GetPlayerConnection(tradeRequest.PlayerRequesting.CreatureId, out var playerRequestingConnection);
        _gameServer.CreatureManager.GetPlayerConnection(tradeRequest.PlayerRequested.CreatureId, out var playerRequestedConnection);

        playerRequestingConnection.OutgoingPackets.Enqueue(new TradeRequestPacket(tradeRequest.PlayerRequesting.Name, tradeRequest.Item));

        var message = $"{tradeRequest.PlayerRequested.Name} wants to trade with you.";
        playerRequestedConnection.OutgoingPackets.Enqueue(new TextMessagePacket(message, TextMessageOutgoingType.Small));

        SendTradeItemToBothPlayers(tradeRequest, playerRequestingConnection, playerRequestedConnection);

        playerRequestingConnection.Send();
        playerRequestedConnection.Send();
    }

    private static void SendTradeItemToBothPlayers(TradeRequest tradeRequest, IConnection playerRequestingConnection,
        IConnection playerRequestedConnection)
    {
        if (!tradeRequest.PlayerAcknowledgedTrade) return;
        
        playerRequestingConnection.OutgoingPackets.Enqueue(new TradeRequestPacket(tradeRequest.PlayerRequested.Name,
            tradeRequest.PlayerRequested.LastTradeRequest.Item, acknowledged: true));
        
        playerRequestedConnection.OutgoingPackets.Enqueue(new TradeRequestPacket(tradeRequest.PlayerRequesting.Name,
            tradeRequest.Item, acknowledged: true));
    }
}