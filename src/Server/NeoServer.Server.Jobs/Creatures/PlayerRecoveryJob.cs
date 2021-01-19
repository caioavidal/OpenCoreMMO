using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Jobs.Creatures
{
    public class PlayerRecoveryJob
    {
        public static void Execute(IPlayer player, Game game)
        {
            if (player.IsDead) return;
            if (!player.Recovering) return;

            player.Recover();
        }

    }
}
