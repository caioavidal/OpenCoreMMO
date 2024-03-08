﻿using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Application.Features.Creature.Monster.Routines;

public static class MonsterDefenseJob
{
    public static void Execute(IMonster monster, IGameServer game)
    {
        if (monster.IsDead) return;

        if (!monster.IsInCombat || monster.Defending) return;

        var interval = monster.Defend();

        ScheduleDefense(game, monster, interval);
    }

    private static void ScheduleDefense(IGameServer game, IMonster monster, ushort interval)
    {
        if (monster.Defending)
            game.Scheduler.AddEvent(new SchedulerEvent(interval, () =>
            {
                var defendInterval = monster.Defend();
                ScheduleDefense(game, monster, defendInterval);
            }));
    }
}