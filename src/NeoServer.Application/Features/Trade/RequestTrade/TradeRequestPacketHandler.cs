using Mediator;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Features.Shared;
using NeoServer.Infrastructure.Thread;
using NeoServer.Networking.Packets.Incoming.Trade;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Tasks;

namespace NeoServer.Application.Features.Trade.RequestTrade;

public class TradeRequestPacketHandler : PacketHandler
{
    private readonly IDispatcher _dispatcher;
    private readonly IGameCreatureManager _gameCreatureManager;
    private readonly ItemFinder _itemFinder;
    private readonly IMediator _mediator;
    private readonly TradeRequestCommand _tradeRequestCommand;

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