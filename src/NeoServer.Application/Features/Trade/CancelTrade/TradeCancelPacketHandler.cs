using NeoServer.Application.Common.PacketHandler;
using NeoServer.Infrastructure.Thread;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Tasks;

namespace NeoServer.Application.Features.Trade.CancelTrade;

public class TradeCancelPacketHandler : PacketHandler
{
    private readonly IDispatcher _dispatcher;
    private readonly IGameCreatureManager _gameCreatureManager;
    private readonly SafeTradeSystem _tradeSystem;

    public TradeCancelPacketHandler(SafeTradeSystem tradeSystem, IGameCreatureManager gameCreatureManager,
        IDispatcher dispatcher)
    {
        _tradeSystem = tradeSystem;
        _gameCreatureManager = gameCreatureManager;
        _dispatcher = dispatcher;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!_gameCreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (player is null) return;

        _dispatcher.AddEvent(new Event(() =>
            _tradeSystem.Cancel(player)));
    }
}