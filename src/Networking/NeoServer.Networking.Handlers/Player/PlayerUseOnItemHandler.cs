using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerUseOnItemHandler : PacketHandler
    {
        private readonly IGameServer game;
        public PlayerUseOnItemHandler(IGameServer game)
        {
            this.game = game;
        }
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var useItemOnPacket = new UseItemOnPacket(message);
            if (game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            {
                game.Dispatcher.AddEvent(new Event(2000, new PlayerUseItemOnCommand(player, game, useItemOnPacket).Execute)); //todo create a const for 2000 expiration time
            }
        }
    }
}
