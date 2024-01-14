using NeoServer.Application.Common.PacketHandler;
using NeoServer.Infrastructure.Thread;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Tasks;

namespace NeoServer.Application.Features.Trade;

public class TradeAcceptHandler : PacketHandler
{
    private readonly IGameCreatureManager _creatureManager;
    private readonly IDispatcher _dispatcher;
    private readonly SafeTradeSystem _tradeSystem;

    public TradeAcceptHandler(SafeTradeSystem tradeSystem, IGameCreatureManager creatureManager,
        IDispatcher dispatcher)
    {
        _tradeSystem = tradeSystem;
        _creatureManager = creatureManager;
        _dispatcher = dispatcher;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!_creatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (player is null) return;

        _dispatcher.AddEvent(new Event(() => _tradeSystem.AcceptTrade(player)));
    }
}