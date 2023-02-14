using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.World.Models.Spawns;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Jobs.Creatures.Npc;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Jobs.Creatures;

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
            if (creature is null or ICombatActor { IsDead: true }) continue;

            CheckPlayer(creature);

            CheckCreature(creature);

            CheckMonster(creature);

            CheckNpc(creature);

            RespawnJob.Execute(_spawnManager);
        }
    }

    private static void CheckCreature(ICreature creature)
    {
        if (creature is ICombatActor combatActor) CreatureConditionJob.Execute(combatActor);
    }

    private static void CheckNpc(ICreature creature)
    {
        if (creature is INpc npc) NpcJob.Execute(npc);
    }

    private void CheckMonster(ICreature creature)
    {
        if (creature is not IMonster monster) return;

        CreatureDefenseJob.Execute(monster, _game);
        MonsterStateJob.Execute(monster, _summonService);
        MonsterYellJob.Execute(monster);
    }

    private void CheckPlayer(ICreature creature)
    {
        if (creature is not IPlayer player) return;

        PlayerPingJob.Execute(player, _playerLogOutCommand, _game);
        PlayerRecoveryJob.Execute(player);
    }
}