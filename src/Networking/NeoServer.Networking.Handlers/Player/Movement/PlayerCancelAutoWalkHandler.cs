using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Players
{
    public class PlayerCancelAutoWalkHandler : PacketHandler
    {
        private readonly IGameServer game;
        private readonly IMap map;

        public PlayerCancelAutoWalkHandler(IGameServer game, IMap map)
        {
            this.game = game;
            this.map = map;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (game.CreatureManager.TryGetPlayer(connection.CreatureId, out IPlayer player))
            {
                game.Dispatcher.AddEvent(new Event(player.CancelWalk));
            }
        }
    }
}
