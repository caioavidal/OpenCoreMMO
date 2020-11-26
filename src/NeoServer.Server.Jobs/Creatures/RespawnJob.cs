using NeoServer.Game.World.Spawns;
using System;

namespace NeoServer.Server.Jobs.Creatures
{
    public class RespawnJob
    {
        private const int INTERVAL = 10000;
        private static long _lastRespawn = 0;
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
}
