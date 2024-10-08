using NeoServer.Game.Combat.Attacks.MeleeAttack;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Results;

namespace NeoServer.Application.Features.Combat.Attacks.MeleeAttack;

public sealed class MeleeAttackValidation(AttackValidation attackValidation) : IAttackValidation
{
    public Result Validate(ICombatActor aggressor, ICombatActor victim)
    {
        ArgumentNullException.ThrowIfNull(aggressor);

        if (victim is null) return Result.NotPossible;

        var result = attackValidation.Validate(aggressor, victim);
        if (result.Failed) return result;
        
        var aggressorIsNextToVictim = aggressor.Location.IsNextTo(victim.Location);
        
        return aggressorIsNextToVictim ? Result.Success : Result.Fail(InvalidOperation.TooFar);
    }
}