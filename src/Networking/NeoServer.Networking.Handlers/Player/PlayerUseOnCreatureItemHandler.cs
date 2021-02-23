using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerUseOnCreatureHandler : PacketHandler
    {
        private readonly IGameServer game;
        public PlayerUseOnCreatureHandler(IGameServer game)
        {
            this.game = game;
        }
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var useItemOnPacket = new UseItemOnCreaturePacket(message);
            if (game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            {
                game.Dispatcher.AddEvent(new Event(2000, new PlayerUseItemOnCreatureCommand(player, game, useItemOnPacket).Execute)); //todo create a const for 2000 expiration time
            }
        }
    }
}
