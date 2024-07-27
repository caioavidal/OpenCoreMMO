﻿using System;
using NeoServer.Application.Common;
using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Creature.Player;

namespace NeoServer.Extensions.Spells.Commands;

public class AttrPlayerCommand : CommandSpell
{
    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        error = InvalidOperation.NotPossible;

        if (NumberOfParams != 2)
            return false;

        var ctx = IoC.GetInstance<IGameCreatureManager>();
        ctx.TryGetPlayer(Params[0].ToString(), out var player);

        if (player is null)
            return false;

        if (!int.TryParse((string)Params[1], out var level))
            return false;

        AdjustExperience(player, level);

        return true;
    }

    private static void AdjustExperience(IPlayer player, int level)
    {
        if (level < 0)
        {
            var expTarget = Skill.CalculateExpByLevel(player.Level - Math.Abs(level));
            player.LoseExperience((long)expTarget);
            return;
        }

        var expForNewLevel = Skill.CalculateExpByLevel(player.Level + level);
        player.GainExperience((long)(expForNewLevel - player.Experience));
    }
}