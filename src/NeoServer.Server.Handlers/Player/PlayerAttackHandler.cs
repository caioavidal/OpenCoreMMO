using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerAttackHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerAttackHandler(Game game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var targetId = message.GetUInt32();
            if (game.CreatureManager.TryGetCreature(connection.PlayerId, out ICreature player))
            {
                player.SetAttackTarget(targetId);
            }
        }
    }
}
