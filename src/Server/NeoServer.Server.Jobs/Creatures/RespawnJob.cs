using System;
using NeoServer.Game.World.Models.Spawns;

namespace NeoServer.Server.Jobs.Creatures;

public class RespawnJob
{
    private const int INTERVAL = 10000;
    private static long _lastRespawn;

    public static void Execute(SpawnManager spawnManager)
    {
        var now = DateTime.Now.Ticks;
        var remainingTime = TimeSpan.FromTicks(now - _lastRespawn).TotalMilliseconds;

        if (remainingTime >= INTERVAL)
        {
            spawnManager.Respawn();
            _lastRespawn = now;
        }
    }
}