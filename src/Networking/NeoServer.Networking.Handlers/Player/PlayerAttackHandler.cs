using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerAttackHandler : PacketHandler
    {
        private readonly IGameServer game;

        public PlayerAttackHandler(IGameServer game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var targetId = message.GetUInt32();

            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            if (targetId == 0)
            {
                game.Dispatcher.AddEvent(new Event(() => player.StopAttack()));
                return;
            }

            if (!game.CreatureManager.TryGetCreature(targetId, out var creature)) return;

            game.Dispatcher.AddEvent(new Event(() =>
                player.SetAttackTarget(creature)));
        }
    }
}