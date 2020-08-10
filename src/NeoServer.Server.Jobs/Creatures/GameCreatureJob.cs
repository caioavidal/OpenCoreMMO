using NeoServer.Game.World.Spawns;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Jobs.Creatures
{
    public class GameCreatureJob
    {
        private const ushort EVENT_CHECK_CREATURE_INTERVAL = 100;
        private readonly Game game;
        private readonly SpawnManager spawnManager;

        public GameCreatureJob(Game game, SpawnManager spawnManager)
        {
            this.game = game;
            this.spawnManager = spawnManager;
        }



        public void StartCheckingCreatures()
        {
            game.Scheduler.AddEvent(new SchedulerEvent(EVENT_CHECK_CREATURE_INTERVAL, StartCheckingCreatures));


            foreach (var creature in game.CreatureManager.GetCreatures())
            {
                if (creature.IsDead)
                {
                    continue;
                }
                if (creature is IPlayer)
                {
                    PlayerPingJob.Execute((IPlayer)creature, game);
                }

                CreatureAttakingJob.Execute(creature, game);
                RespawnJob.Execute(spawnManager);
            }

        }
    }
}
