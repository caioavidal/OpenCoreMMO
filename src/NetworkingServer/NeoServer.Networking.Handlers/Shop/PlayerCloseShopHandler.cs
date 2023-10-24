using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Tasks;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Shop;

public class PlayerCloseShopHandler : PacketHandler
{
    private readonly IGameCreatureManager _gameCreatureManager;
    private readonly IDispatcher _dispatcher;

    public PlayerCloseShopHandler(IGameCreatureManager gameCreatureManager, IDispatcher dispatcher)
    {
        _gameCreatureManager = gameCreatureManager;
        _dispatcher = dispatcher;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!_gameCreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
        _dispatcher.AddEvent(new Event(() => player.StopShopping()));
    }
}