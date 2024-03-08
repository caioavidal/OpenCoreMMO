﻿using NeoServer.Game.Combat.Conditions;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Application.Features.Creature;

public static class CreatureConditionRoutine
{
    public static void Execute(ICombatActor creature)
    {
        if (creature.IsDead) return;

        foreach (var (_, condition) in creature.Conditions.ToList())
        {
            if (condition.HasExpired)
            {
                condition.End();
                creature.RemoveCondition(condition);
            }

            if (condition is DamageCondition damageCondition) damageCondition.Execute(creature);
        }
    }
}