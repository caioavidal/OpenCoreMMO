using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
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

            if (game.CreatureManager.TryGetCreature(connection.PlayerId, out ICreature player))
            {
                game.Dispatcher.AddEvent(new Event(() => (player as IPlayer).Containers.GoBackContainer(containerId))); //todo create a const for 2000 expiration time
            }
        }
    }
}
