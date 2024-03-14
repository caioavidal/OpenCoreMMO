using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Networking.Packets.Incoming.Shop;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.Application.Features.Shop.Sell;

public class PlayerSalePacketHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly IItemClientServerIdMapStore _itemClientServerIdMapStore;
    private readonly IItemTypeStore _itemTypeStore;

    public PlayerSalePacketHandler(IGameServer game, IItemTypeStore itemTypeStore,
        IItemClientServerIdMapStore itemClientServerIdMapStore)
    {
        _game = game;
        _itemTypeStore = itemTypeStore;
        _itemClientServerIdMapStore = itemClientServerIdMapStore;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var playerSalePacket = new PlayerSalePacket(message);
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        var serverId = _itemClientServerIdMapStore.Get(playerSalePacket.ItemClientId);

        if (!_itemTypeStore.TryGetValue(serverId, out var itemType)) return;

        _game.Dispatcher.AddEvent(new Event(() =>
            player.Sell(itemType, playerSalePacket.Amount, playerSalePacket.IgnoreEquipped)));
    }
}