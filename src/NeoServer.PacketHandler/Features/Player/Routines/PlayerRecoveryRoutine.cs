using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.PacketHandler.Features.Player.Routines;

public static class PlayerRecoveryRoutine
{
    public static void Execute(IPlayer player)
    {
        if (player.IsDead) return;
        if (!player.Recovering) return;

        player.Recover();
    }
}