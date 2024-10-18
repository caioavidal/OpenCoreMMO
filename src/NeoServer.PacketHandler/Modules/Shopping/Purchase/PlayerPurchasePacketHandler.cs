using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Dispatcher;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Networking.Packets.Incoming.Shop;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Shopping.Purchase;

public class PlayerPurchasePacketHandler : PacketHandler
{
    private readonly IGameCreatureManager _creatureManager;
    private readonly IDealTransaction _dealTransaction;
    private readonly IDispatcher _dispatcher;
    private readonly IItemClientServerIdMapStore _itemClientServerIdMapStore;
    private readonly IItemTypeStore _itemTypeStore;

    public PlayerPurchasePacketHandler(IDealTransaction dealTransaction, IItemTypeStore itemTypeStore,
        IGameCreatureManager creatureManager, IDispatcher dispatcher,
        IItemClientServerIdMapStore itemClientServerIdMapStore)
    {
        _dealTransaction = dealTransaction;
        _itemTypeStore = itemTypeStore;
        _creatureManager = creatureManager;
        _dispatcher = dispatcher;
        _itemClientServerIdMapStore = itemClientServerIdMapStore;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var playerPurchasePacket = new PlayerPurchasePacket(message);
        if (!_creatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        var serverId = _itemClientServerIdMapStore.Get(playerPurchasePacket.ItemClientId);

        if (!_itemTypeStore.TryGetValue(serverId, out var itemType)) return;

        _dispatcher.AddEvent(new Event(() =>
            _dealTransaction?.Buy(player, player.TradingWithNpc, itemType, playerPurchasePacket.Amount)));
    }
}