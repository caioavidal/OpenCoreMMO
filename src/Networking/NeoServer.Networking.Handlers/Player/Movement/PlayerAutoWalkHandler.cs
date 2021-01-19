using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Players
{
    public class PlayerAutoWalkHandler : PacketHandler
    {
        private readonly Game game;

        public PlayerAutoWalkHandler(Game game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var autoWalk = new AutoWalkPacket(message);

            if (game.CreatureManager.TryGetPlayer(connection.PlayerId, out IPlayer player))
            {
                game.Dispatcher.AddEvent(new Event(() => player.TryWalkTo(autoWalk.Steps)));
            }
        }
    }
}
