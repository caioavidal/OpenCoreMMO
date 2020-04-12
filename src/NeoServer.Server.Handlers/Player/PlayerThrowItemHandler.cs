using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerThrowItemHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerThrowItemHandler(Game game)
        {
            this.game = game;
        }
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var itemThrowPacket = new ItemThrowPacket(message);
            if (game.CreatureManager.TryGetCreature(connection.PlayerId, out ICreature player))
            {
                game.Dispatcher.AddEvent(new Event(2000, new PlayerThrowItemCommand((IPlayer)player, itemThrowPacket, game).Execute)); //todo create a const for 2000 expiration time
            }
        }
    }
}
