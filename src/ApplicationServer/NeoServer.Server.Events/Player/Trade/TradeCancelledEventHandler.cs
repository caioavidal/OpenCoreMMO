using NeoServer.Game.Creatures.Trade.Request;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Trade;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Server.Events.Player.Trade;

public class TradeCancelledEventHandler: IEventHandler
{
    private readonly IGameServer _gameServer;

    public TradeCancelledEventHandler(IGameServer gameServer)
    {
        _gameServer = gameServer;
    }
    public void Execute(TradeRequest tradeRequest)
    {
        _gameServer.CreatureManager.GetPlayerConnection(tradeRequest.FirstPlayer, out var firstPlayerConnection);
        _gameServer.CreatureManager.GetPlayerConnection(tradeRequest.SecondPlayer, out var secondPlayerConnection);

        SendMessage(firstPlayerConnection, secondPlayerConnection);
        SendTradeClosePacket(firstPlayerConnection, secondPlayerConnection);
        
        firstPlayerConnection.Send();
        secondPlayerConnection.Send();
    }

    private static void SendTradeClosePacket(IConnection firstPlayerConnection, IConnection secondPlayerConnection)
    {
        firstPlayerConnection.OutgoingPackets.Enqueue(new TradeClosePacket());
        secondPlayerConnection.OutgoingPackets.Enqueue(new TradeClosePacket());
    }

    private static void SendMessage(IConnection firstPlayerConnection, IConnection secondPlayerConnection)
    {
        firstPlayerConnection?.OutgoingPackets.Enqueue(new TextMessagePacket("Trade is cancelled.",
            TextMessageOutgoingType.Small));
        
        secondPlayerConnection?.OutgoingPackets.Enqueue(new TextMessagePacket("Trade is cancelled.",
            TextMessageOutgoingType.Small));
    }
}