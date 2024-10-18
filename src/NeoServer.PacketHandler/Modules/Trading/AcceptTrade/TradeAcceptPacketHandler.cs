using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Dispatcher;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Modules.Trading;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Trading.AcceptTrade;

public class TradeAcceptPacketHandler : PacketHandler
{
    private readonly IGameCreatureManager _creatureManager;
    private readonly IDispatcher _dispatcher;
    private readonly SafeTradeSystem _tradeSystem;

    public TradeAcceptPacketHandler(SafeTradeSystem tradeSystem, IGameCreatureManager creatureManager,
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