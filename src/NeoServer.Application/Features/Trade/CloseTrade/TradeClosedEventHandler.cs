using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Features.Trade.RequestTrade;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing.Trade;

namespace NeoServer.Application.Features.Trade.CloseTrade;

public class TradeClosedEventHandler : IEventHandler
{
    private readonly IGameServer _gameServer;

    public TradeClosedEventHandler(IGameServer gameServer)
    {
        _gameServer = gameServer;
    }

    public void Execute(TradeRequest tradeRequest)
    {
        _gameServer.CreatureManager.GetPlayerConnection(tradeRequest.PlayerRequesting.CreatureId,
            out var firstPlayerConnection);
        _gameServer.CreatureManager.GetPlayerConnection(tradeRequest.PlayerRequested.CreatureId,
            out var secondPlayerConnection);

        SendTradeClosePacket(firstPlayerConnection, secondPlayerConnection);

        firstPlayerConnection?.Send();
        secondPlayerConnection?.Send();
    }

    private static void SendTradeClosePacket(IConnection firstPlayerConnection, IConnection secondPlayerConnection)
    {
        firstPlayerConnection?.OutgoingPackets.Enqueue(new TradeClosePacket());
        secondPlayerConnection?.OutgoingPackets.Enqueue(new TradeClosePacket());
    }
}