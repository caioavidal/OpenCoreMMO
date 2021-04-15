using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Players
{
    public class PlayerAutoWalkHandler : PacketHandler
    {
        private readonly IGameServer game;

        public PlayerAutoWalkHandler(IGameServer game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var autoWalk = new AutoWalkPacket(message);

            if (game.CreatureManager.TryGetPlayer(connection.CreatureId, out IPlayer player))
            {
                game.Dispatcher.AddEvent(new Event(() => player.WalkTo(autoWalk.Steps)));
            }
        }
    }
}
