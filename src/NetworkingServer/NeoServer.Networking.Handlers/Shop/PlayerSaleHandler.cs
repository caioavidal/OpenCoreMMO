using NeoServer.Data.InMemory.DataStores;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Networking.Packets.Incoming.Shop;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Shop;

public class PlayerSaleHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly IItemTypeStore _itemTypeStore;

    public PlayerSaleHandler(IGameServer game, IItemTypeStore itemTypeStore)
    {
        _game = game;
        _itemTypeStore = itemTypeStore;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var playerSalePacket = new PlayerSalePacket(message);
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        var serverId = ItemClientServerIdMapStore.Data.Get(playerSalePacket.ItemClientId);

        if (!_itemTypeStore.TryGetValue(serverId, out var itemType)) return;

        _game.Dispatcher.AddEvent(new Event(() =>
            player.Sell(itemType, playerSalePacket.Amount, playerSalePacket.IgnoreEquipped)));
    }
}