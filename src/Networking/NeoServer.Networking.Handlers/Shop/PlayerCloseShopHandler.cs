using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Shop
{
    public class PlayerCloseShopHandler : PacketHandler
    {
        private readonly IGameServer game;

        public PlayerCloseShopHandler(IGameServer game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
            game.Dispatcher.AddEvent(new Event(() => player.StopShopping()));
        }
    }
}