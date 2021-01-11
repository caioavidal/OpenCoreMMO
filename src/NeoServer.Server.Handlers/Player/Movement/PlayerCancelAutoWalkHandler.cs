using NeoServer.Game.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Players
{
    public class PlayerCancelAutoWalkHandler : PacketHandler
    {
        private readonly Game game;
        private readonly IMap map;

        public PlayerCancelAutoWalkHandler(Game game, IMap map)
        {
            this.game = game;
            this.map = map;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (game.CreatureManager.TryGetPlayer(connection.PlayerId, out IPlayer player))
            {
                game.Dispatcher.AddEvent(new Event(player.CancelWalk));
            }
        }
    }
}
