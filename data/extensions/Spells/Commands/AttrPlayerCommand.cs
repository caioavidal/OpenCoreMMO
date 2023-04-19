using System;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Creatures.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Helpers;

namespace NeoServer.Extensions.Spells.Commands;

public class AttrPlayerCommand : CommandSpell
{
    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        error = InvalidOperation.NotPossible;
        
        if (Params.Length != 2)
            return false;

        var ctx = IoC.GetInstance<IGameCreatureManager>();
        ctx.TryGetPlayer(Params[0].ToString(), out var player);

        if (player is null)
            return false;

        if (!int.TryParse((string)Params[1], out var level))
            return false;

        if (level >= 0)
        {
            var expForNewLevel = Skill.CalculateExpByLevel(player.Level + level);
        
            player.GainExperience((long)(expForNewLevel - player.Experience));
        }
        else
        {
            var expTarget = Skill.CalculateExpByLevel(player.Level - Math.Abs(level));
            player.LoseExperience((long)expTarget);
        }

        return true;
    }
}