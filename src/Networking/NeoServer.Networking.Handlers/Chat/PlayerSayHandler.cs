using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerSayHandler : PacketHandler
    {
        private readonly IGameServer game;
        public PlayerSayHandler(IGameServer game)
        {
            this.game = game;
        }
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var playerSay = new PlayerSayPacket(message);
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            game.Dispatcher.AddEvent(new Event(() => new PlayerSayCommand(player,connection, playerSay, game).Execute()));

        }
    }
}
