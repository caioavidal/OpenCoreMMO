using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Authentication
{
    public class PlayerLogOutHandler : PacketHandler
    {
        private readonly Game game;

        public PlayerLogOutHandler(Game game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (game.CreatureManager.TryGetCreature(connection.PlayerId, out ICreature player))
            {
                game.Dispatcher.AddEvent(new Event(new PlayerLogOutCommand((IPlayer)player, game).Execute));
            }
        }
    }
}
