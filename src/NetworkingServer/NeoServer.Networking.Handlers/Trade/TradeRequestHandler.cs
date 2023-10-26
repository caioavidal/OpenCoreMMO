using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Networking.Packets.Incoming.Trade;
using NeoServer.Server.Commands.Trade;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Tasks;

namespace NeoServer.Networking.Handlers.Trade;

public class TradeRequestHandler : PacketHandler
{
    private readonly TradeRequestCommand _tradeRequestCommand;
    private readonly IGameCreatureManager _gameCreatureManager;
    private readonly IDispatcher _dispatcher;

    public TradeRequestHandler(TradeRequestCommand tradeRequestCommand, IGameCreatureManager gameCreatureManager, IDispatcher dispatcher)
    {
        _tradeRequestCommand = tradeRequestCommand;
        _gameCreatureManager = gameCreatureManager;
        _dispatcher = dispatcher;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var tradeRequestPacket = new TradeRequestPacket(message);
        if (!_gameCreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (player is null) return;

        _dispatcher.AddEvent(new Event(2000,
            () => _tradeRequestCommand.RequestTrade(player, tradeRequestPacket)));
    }
}