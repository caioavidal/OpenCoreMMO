using NeoServer.Application.Common;
using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Repositories;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Extensions.Spells.Commands;

public class BanPlayerCommand : CommandSpell
{
    private const string BANISH_REASON = "You have been banished by a gamemaster.";

    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        error = InvalidOperation.NotPossible;

        if (Params.Length == 0)
            return false;

        var ctx = IoC.GetInstance<IGameCreatureManager>();
        var accountRepository = IoC.GetInstance<IAccountRepository>();

        if (!ctx.TryGetPlayer(Params[0].ToString(), out var player))
            return false;

        if (player is null || player.CreatureId == actor.CreatureId)
            return false;

        var reason = Params[1]?.ToString() ?? BANISH_REASON;

        accountRepository.Ban(player.AccountId, reason, ((IPlayer)actor).AccountId).Wait();
        player.Logout(true);

        return true;
    }
}