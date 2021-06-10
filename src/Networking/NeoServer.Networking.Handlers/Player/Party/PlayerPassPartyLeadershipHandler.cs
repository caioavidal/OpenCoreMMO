using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player.Party
{
    public class PlayerPassPartyLeadershipHandler : PacketHandler
    {
        private readonly IGameServer game;

        public PlayerPassPartyLeadershipHandler(IGameServer game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var targetCreatureId = message.GetUInt32();
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
            if (!game.CreatureManager.TryGetPlayer(targetCreatureId, out var targetPlayer) ||
                !game.CreatureManager.IsPlayerLogged(targetPlayer))
            {
                connection.Send(new TextMessagePacket("Player is not online.", TextMessageOutgoingType.Small));
                return;
            }

            game.Dispatcher.AddEvent(new Event(() => player.PassPartyLeadership(targetPlayer)));
        }
    }
}