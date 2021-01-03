using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerSayHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerSayHandler(Game game)
        {
            this.game = game;
        }
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var playerSay = new PlayerSayPacket(message);
            if (!game.CreatureManager.TryGetPlayer(connection.PlayerId, out var player)) return;
            
            game.Dispatcher.AddEvent(new Event(() => player.Say(playerSay.Message, playerSay.Talk)));
            
        }
    }
}
