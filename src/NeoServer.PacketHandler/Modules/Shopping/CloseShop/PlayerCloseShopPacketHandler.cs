using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Dispatcher;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Shopping.CloseShop;

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