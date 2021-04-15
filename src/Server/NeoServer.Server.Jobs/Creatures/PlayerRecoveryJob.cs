using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts;

namespace NeoServer.Server.Jobs.Creatures
{
    public class PlayerRecoveryJob
    {
        public static void Execute(IPlayer player, IGameServer game)
        {
            if (player.IsDead) return;
            if (!player.Recovering) return;

            player.Recover();
        }

    }
}
