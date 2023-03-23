using NeoServer.Game.Creatures.Trade.Request;
using NeoServer.Networking.Packets.Outgoing.Trade;
using NeoServer.Server.Common.Contracts;

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
        _gameServer.CreatureManager.GetPlayerConnection(tradeRequest.FirstPlayer, out var firstPlayerConnection);
        _gameServer.CreatureManager.GetPlayerConnection(tradeRequest.SecondPlayer, out var secondPlayerConnection);

        _gameServer.CreatureManager.TryGetPlayer(tradeRequest.FirstPlayer, out var firstPlayer);

        firstPlayerConnection.OutgoingPackets.Enqueue(new TradeRequestPacket(firstPlayer.Name, tradeRequest.ItemFromFirstPlayer));
        
        firstPlayerConnection.Send();
    }
}