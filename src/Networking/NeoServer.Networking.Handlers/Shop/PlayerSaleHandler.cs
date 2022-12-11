using NeoServer.Data.InMemory.DataStores;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Networking.Packets.Incoming.Shop;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Shop;

public class PlayerSaleHandler : PacketHandler
{
    private readonly IItemTypeStore _itemTypeStore;
    private readonly IGameServer game;

    public PlayerSaleHandler(IGameServer game, IItemTypeStore itemTypeStore)
    {
        this.game = game;
        _itemTypeStore = itemTypeStore;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var playerSalePacket = new PlayerSalePacket(message);
        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        var serverId = ItemClientServerIdMapStore.Data.Get(playerSalePacket.ItemClientId);

        if (!_itemTypeStore.TryGetValue(serverId, out var itemType)) return;

        game.Dispatcher.AddEvent(new Event(() =>
            player.Sell(itemType, playerSalePacket.Amount, playerSalePacket.IgnoreEquipped)));
    }
}