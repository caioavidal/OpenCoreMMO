using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.World.Models.Spawns;
using NeoServer.Server.Commands;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Jobs.Creatures.Npc;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Jobs.Creatures
{
    public class GameCreatureJob
    {
        private const ushort EVENT_CHECK_CREATURE_INTERVAL = 500;
        private readonly IGameServer _game;
        private readonly PlayerLogOutCommand _playerLogOutCommand;
        private readonly SpawnManager _spawnManager;
        private readonly ISummonService _summonService;

        public GameCreatureJob(IGameServer game, SpawnManager spawnManager, PlayerLogOutCommand playerLogOutCommand,
            ISummonService summonService)
        {
            _game = game;
            _spawnManager = spawnManager;
            _playerLogOutCommand = playerLogOutCommand;
            _summonService = summonService;
        }

        public void StartChecking()
        {
            _game.Scheduler.AddEvent(new SchedulerEvent(EVENT_CHECK_CREATURE_INTERVAL, StartChecking));

            foreach (var creature in _game.CreatureManager.GetCreatures())
            {
                if (creature is ICombatActor {IsDead: true}) continue;
                if (creature is null) continue;

                if (creature is IPlayer player)
                {
                    PlayerPingJob.Execute(player, _playerLogOutCommand, _game);
                    PlayerRecoveryJob.Execute(player, _game);
                    PlayerItemJob.Execute(player);
                }

                if (creature is ICombatActor combatActor) CreatureConditionJob.Execute(combatActor);

                if (creature is IMonster monster)
                {
                    CreatureDefenseJob.Execute(monster, _game);
                    MonsterStateJob.Execute(monster, _summonService);
                    MonsterYellJob.Execute(monster);
                }

                if (creature is INpc npc) NpcJob.Execute(npc);

                RespawnJob.Execute(_spawnManager);
            }
        }
    }
}