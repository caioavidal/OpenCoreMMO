using NeoServer.Game.Systems.SafeTrade;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Tasks;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Trade;

public class TradeCancelHandler : PacketHandler
{
    
    private readonly SafeTradeSystem _tradeSystem;
    private readonly IGameCreatureManager _gameCreatureManager;
    private readonly IDispatcher _dispatcher;

    public TradeCancelHandler(SafeTradeSystem tradeSystem, IGameCreatureManager gameCreatureManager,
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