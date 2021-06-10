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
        private readonly IDealTransaction dealTransaction;
        private readonly IGameServer game;

        public PlayerPurchaseHandler(IGameServer game, IDealTransaction dealTransaction)
        {
            this.game = game;
            this.dealTransaction = dealTransaction;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var playerPurchasePacket = new PlayerPurchasePacket(message);
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            var serverId = ItemIdMapStore.Data.Get(playerPurchasePacket.ItemClientId);

            if (!ItemTypeStore.Data.TryGetValue(serverId, out var itemType)) return;

            game.Dispatcher.AddEvent(new Event(() =>
                dealTransaction?.Buy(player, player.TradingWithNpc, itemType, playerPurchasePacket.Amount)));
        }
    }
}