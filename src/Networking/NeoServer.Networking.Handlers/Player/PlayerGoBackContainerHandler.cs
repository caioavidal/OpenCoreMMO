using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerGoBackContainerHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerGoBackContainerHandler(Game game)
        {
            this.game = game;
        }
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var containerId = message.GetByte();

            if (game.CreatureManager.TryGetPlayer(connection.PlayerId, out var player))
            {
                game.Dispatcher.AddEvent(new Event(() => player.Containers.GoBackContainer(containerId))); //todo create a const for 2000 expiration time
            }
        }
    }
}
