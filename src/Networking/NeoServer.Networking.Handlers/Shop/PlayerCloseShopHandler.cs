using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerCloseShopHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerCloseShopHandler(Game game)
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
