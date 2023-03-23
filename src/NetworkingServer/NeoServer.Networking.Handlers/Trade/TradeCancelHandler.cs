using NeoServer.Game.Creatures.Trade;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Trade;

public class TradeCancelHandler : PacketHandler
{
    private readonly TradeSystem _tradeSystem;
    private readonly IGameServer _gameServer;

    public TradeCancelHandler(TradeSystem tradeSystem, IGameServer gameServer)
    {
        _tradeSystem = tradeSystem;
        _gameServer = gameServer;
    }
    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!_gameServer.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (player is null) return;
        
        _gameServer.Dispatcher.AddEvent(new Event( () => _tradeSystem.CancelTrade(((Game.Creatures.Player.Player)player).LastTradeRequest)));
    }
}