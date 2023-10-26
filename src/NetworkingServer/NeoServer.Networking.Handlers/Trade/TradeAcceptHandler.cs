using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Game.Systems.SafeTrade;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Tasks;

namespace NeoServer.Networking.Handlers.Trade;

public class TradeAcceptHandler : PacketHandler
{
    private readonly SafeTradeSystem _tradeSystem;
    private readonly IGameCreatureManager _creatureManager;
    private readonly IDispatcher _dispatcher;

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