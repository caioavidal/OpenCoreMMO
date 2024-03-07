using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Network;
using NeoServer.Application.Common.Contracts.Tasks;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Shop.CloseShop;

public class PlayerCloseShopPacketHandler : PacketHandler
{
    private readonly IDispatcher _dispatcher;
    private readonly IGameCreatureManager _gameCreatureManager;

    public PlayerCloseShopPacketHandler(IGameCreatureManager gameCreatureManager, IDispatcher dispatcher)
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