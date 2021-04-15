using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.World.Spawns;
using NeoServer.Server.Commands;
using NeoServer.Server.Contracts;
using NeoServer.Server.Jobs.Creatures.Npc;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Jobs.Creatures
{
    public class GameCreatureJob
    {
        private const ushort EVENT_CHECK_CREATURE_INTERVAL = 500;
        private readonly IGameServer game;
        private readonly SpawnManager spawnManager;
        private readonly PlayerLogOutCommand playerLogOutCommand;

        public GameCreatureJob(IGameServer game, SpawnManager spawnManager, PlayerLogOutCommand playerLogOutCommand)
        {
            this.game = game;
            this.spawnManager = spawnManager;
            this.playerLogOutCommand = playerLogOutCommand;
        }

        public void StartChecking()
        {
            game.Scheduler.AddEvent(new SchedulerEvent(EVENT_CHECK_CREATURE_INTERVAL, StartChecking));

            foreach (var creature in game.CreatureManager.GetCreatures()) 
            {
                if (creature is ICombatActor actor && actor.IsDead) continue;
                if (creature is null) continue;

                if (creature is IPlayer player)
                {
                    PlayerPingJob.Execute(player, playerLogOutCommand, game);
                    PlayerRecoveryJob.Execute(player, game);
                }

                if (creature is ICombatActor combatActor)
                {
                    CreatureConditionJob.Execute(combatActor);
                }

                if(creature is IMonster monster)
                {
                    CreatureDefenseJob.Execute(monster, game);
                    MonsterStateJob.Execute(monster);
                    MonsterYellJob.Execute(monster);
                }
                if(creature is INpc npc)
                {
                    NpcJob.Execute(npc);
                }

                RespawnJob.Execute(spawnManager);
            }

        }
    }
}
