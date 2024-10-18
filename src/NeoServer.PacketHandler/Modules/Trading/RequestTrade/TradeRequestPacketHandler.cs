using Mediator;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Domain;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Dispatcher;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Modules.Trading.RequestTrade;
using NeoServer.Networking.Packets.Incoming.Trade;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Trading.RequestTrade;

public class TradeRequestPacketHandler : PacketHandler
{
    private readonly IDispatcher _dispatcher;
    private readonly IGameCreatureManager _gameCreatureManager;
    private readonly ItemFinder _itemFinder;
    private readonly IMediator _mediator;

    public TradeRequestPacketHandler(IGameCreatureManager gameCreatureManager, IDispatcher dispatcher,
        ItemFinder itemFinder, IMediator mediator)
    {
        _gameCreatureManager = gameCreatureManager;
        _dispatcher = dispatcher;
        _itemFinder = itemFinder;
        _mediator = mediator;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var tradeRequestPacket = new TradeRequestPacket(message);
        if (!_gameCreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (player is null) return;

        var item = _itemFinder.Find(player, tradeRequestPacket.Location, tradeRequestPacket.ClientId);
        if (item is null) return;

        _gameCreatureManager.TryGetPlayer(tradeRequestPacket.PlayerId, out var secondPlayer);
        if (secondPlayer is null) return;

        var command = new TradeRequestCommand(player, item, secondPlayer);

        _dispatcher.AddEvent(new Event(2000, () => _ = _mediator.Send(command)));
    }
}