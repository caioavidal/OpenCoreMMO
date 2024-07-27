using NeoServer.Application.Common;
using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Extensions.Spells.Commands;

public class MassKickPlayersCommand : CommandSpell
{
    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        var ctx = IoC.GetInstance<IGameCreatureManager>();

        foreach (var player in ctx.GetAllLoggedPlayers())
        {
            if (player is null || player.CreatureId == actor.CreatureId)
                continue;

            player.Logout(true);
        }

        error = InvalidOperation.None;
        return true;
    }
}