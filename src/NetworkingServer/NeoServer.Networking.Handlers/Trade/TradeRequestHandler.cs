using NeoServer.Networking.Packets.Incoming.Trade;
using NeoServer.Server.Commands.Trade;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Trade;

public class TradeRequestHandler : PacketHandler
{
    private readonly IGameServer _gameServer;
    private readonly TradeRequestCommand _tradeRequestCommand;

    public TradeRequestHandler(IGameServer gameServer, TradeRequestCommand tradeRequestCommand)
    {
        _gameServer = gameServer;
        _tradeRequestCommand = tradeRequestCommand;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var tradeRequestPacket = new TradeRequestPacket(message);
        if (!_gameServer.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (player is null) return;

        _gameServer.Dispatcher.AddEvent(new Event(2000,
            () => _tradeRequestCommand.RequestTrade(player, tradeRequestPacket)));
    }
}