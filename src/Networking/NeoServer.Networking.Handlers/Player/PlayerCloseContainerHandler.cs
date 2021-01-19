using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerCloseContainerHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerCloseContainerHandler(Game game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var containerId = message.GetByte();
            if (!game.CreatureManager.TryGetPlayer(connection.PlayerId, out var player)) return;
            
                game.Dispatcher.AddEvent(new Event(() => player.Containers.CloseContainer(containerId)));
            
        }
    }
}
