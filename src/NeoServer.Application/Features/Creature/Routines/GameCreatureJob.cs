﻿using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Features.Creature.Monster;
using NeoServer.Application.Features.Creature.Monster.Routines;
using NeoServer.Application.Features.Creature.Npc.Routines;
using NeoServer.Application.Features.Player.Ping;
using NeoServer.Application.Features.Player.Routines;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.World.Models.Spawns;

namespace NeoServer.Application.Features.Creature.Routines;

public class GameCreatureJob
{
    private const ushort EVENT_CHECK_CREATURE_INTERVAL = 500;
    private readonly IGameServer _game;
    private readonly SpawnManager _spawnManager;
    private readonly ISummonService _summonService;

    public GameCreatureJob(IGameServer game, SpawnManager spawnManager,
        ISummonService summonService)
    {
        _game = game;
        _spawnManager = spawnManager;
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

            RespawnRoutine.Execute(_spawnManager);
        }
    }

    private static void CheckCreature(ICreature creature)
    {
        if (creature is ICombatActor combatActor) CreatureConditionRoutine.Execute(combatActor);
    }

    private static void CheckNpc(ICreature creature)
    {
        if (creature is INpc npc) NpcRoutine.Execute(npc);
    }

    private void CheckMonster(ICreature creature)
    {
        if (creature is not IMonster monster) return;

        MonsterDefenseJob.Execute(monster, _game);
        MonsterStateRoutine.Execute(monster, _summonService);
        MonsterYellRoutine.Execute(monster);
    }

    private void CheckPlayer(ICreature creature)
    {
        if (creature is not IPlayer player) return;

        PingRoutine.Execute(player, _game);
        PlayerRecoveryRoutine.Execute(player);
    }
}