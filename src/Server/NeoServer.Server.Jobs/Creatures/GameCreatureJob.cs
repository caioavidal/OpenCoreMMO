using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.World.Spawns;
using NeoServer.Server.Contracts;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Jobs.Creatures
{
    public class GameCreatureJob
    {
        private const ushort EVENT_CHECK_CREATURE_INTERVAL = 500;
        private readonly IGameServer game;
        private readonly SpawnManager spawnManager;

        public GameCreatureJob(IGameServer game, SpawnManager spawnManager)
        {
            this.game = game;
            this.spawnManager = spawnManager;
        }

        public void StartChecking()
        {
            game.Scheduler.AddEvent(new SchedulerEvent(EVENT_CHECK_CREATURE_INTERVAL, StartChecking));

            foreach (var creature in game.CreatureManager.GetCreatures()) 
            {
                if (creature is ICombatActor actor && actor.IsDead) continue;

                if (creature is IPlayer player)
                {
                    PlayerPingJob.Execute(player, game);
                    PlayerRecoveryJob.Execute(player, game);
                }

                CreatureConditionJob.Execute(creature as ICombatActor);

                if(creature is IMonster monster)
                {
                    CreatureDefenseJob.Execute(monster, game);
                    MonsterStateJob.Execute(monster);
                    MonsterYellJob.Execute(monster);
                }

                RespawnJob.Execute(spawnManager);
            }

        }
    }
}
