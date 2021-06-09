using NeoServer.Game.DataStore;
using NeoServer.Networking.Packets.Incoming.Shop;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerSaleHandler : PacketHandler
    {
        private readonly IGameServer game;

        public PlayerSaleHandler(IGameServer game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var playerSalePacket = new PlayerSalePacket(message);
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            var serverId = ItemIdMapStore.Data.Get(playerSalePacket.ItemClientId);

            if (!ItemTypeStore.Data.TryGetValue(serverId, out var itemType)) return;

            game.Dispatcher.AddEvent(new Event(() =>
                player.Sell(itemType, playerSalePacket.Amount, playerSalePacket.IgnoreEquipped)));
        }
    }
}