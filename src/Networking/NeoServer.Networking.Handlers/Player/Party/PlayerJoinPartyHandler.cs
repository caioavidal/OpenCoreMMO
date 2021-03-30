using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Handlers;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player
{
    public class PlayerJoinPartyHandler : PacketHandler
    {
        private readonly IGameServer game;

        public PlayerJoinPartyHandler(IGameServer game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var leaderId = message.GetUInt32();
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
            if (!game.CreatureManager.TryGetPlayer(leaderId, out var leader) || !game.CreatureManager.IsPlayerLogged(leader))
            {
                connection.Send(new TextMessagePacket("Player is not online.", TextMessageOutgoingType.Small));
                return;
            }

            game.Dispatcher.AddEvent(new Event(() => player.JoinParty(leader.Party)));
        }
    }
}
