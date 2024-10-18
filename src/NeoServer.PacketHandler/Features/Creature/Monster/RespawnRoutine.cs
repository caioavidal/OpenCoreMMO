using NeoServer.Game.World.Models.Spawns;

namespace NeoServer.PacketHandler.Features.Creature.Monster;

public static class RespawnRoutine
{
    private const int INTERVAL = 10000;
    private static long _lastRespawn;

    public static void Execute(SpawnManager spawnManager)
    {
        var now = DateTime.Now.Ticks;
        var remainingTime = TimeSpan.FromTicks(now - _lastRespawn).TotalMilliseconds;

        if (!(remainingTime >= INTERVAL)) return;

        spawnManager.Respawn();
        _lastRespawn = now;
    }
}