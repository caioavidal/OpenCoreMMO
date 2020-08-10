using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.World.Spawns;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Jobs.Creatures
{
    public class RespawnJob
    {
        private const int INTERVAL = 20000;
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
