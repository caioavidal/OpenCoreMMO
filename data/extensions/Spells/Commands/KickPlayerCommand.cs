using NeoServer.Application.Common;
using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Extensions.Spells.Commands;

public class KickPlayerCommand : CommandSpell
{
    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        var commands = words.Split("/kick");

        if (string.IsNullOrWhiteSpace(commands[1]))
        {
            error = InvalidOperation.NotPossible;
            return false;
        }

        var ctx = IoC.GetInstance<IGameCreatureManager>();

        if (!ctx.TryGetPlayer(commands[1], out var player))
        {
            error = InvalidOperation.NotPossible;
            return false;
        }

        if (player is null || player.CreatureId == actor.CreatureId)
        {
            error = InvalidOperation.NotPossible;
            return false;
        }

        player.Logout();

        error = InvalidOperation.None;
        return true;
    }
}