using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.DataStore;
using NeoServer.Networking.Packets.Incoming.Shop;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Shop
{
    public class PlayerPurchaseHandler : PacketHandler
    {
        private readonly IDealTransaction _dealTransaction;
        private readonly IGameServer _game;
        private readonly ItemTypeStore _itemTypeStore;

        public PlayerPurchaseHandler(IGameServer game, IDealTransaction dealTransaction, ItemTypeStore itemTypeStore)
        {
            _game = game;
            _dealTransaction = dealTransaction;
            _itemTypeStore = itemTypeStore;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var playerPurchasePacket = new PlayerPurchasePacket(message);
            if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            var serverId = ItemIdMapStore.Data.Get(playerPurchasePacket.ItemClientId);

            if (!_itemTypeStore.TryGetValue(serverId, out var itemType)) return;

            _game.Dispatcher.AddEvent(new Event(() =>
                _dealTransaction?.Buy(player, player.TradingWithNpc, itemType, playerPurchasePacket.Amount)));
        }
    }
}