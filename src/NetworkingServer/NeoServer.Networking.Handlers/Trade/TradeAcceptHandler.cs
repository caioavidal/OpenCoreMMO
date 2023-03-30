using NeoServer.Game.Creatures.Trade;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Trade;

public class TradeAcceptHandler : PacketHandler
{
    private readonly IGameServer _gameServer;
    private readonly TradeSystem _tradeSystem;

    public TradeAcceptHandler(TradeSystem tradeSystem, IGameServer gameServer)
    {
        _tradeSystem = tradeSystem;
        _gameServer = gameServer;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!_gameServer.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (player is null) return;

        _gameServer.Dispatcher.AddEvent(new Event(() => _tradeSystem.AcceptTrade(player)));
    }
}