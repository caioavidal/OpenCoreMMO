using NeoServer.Application.Features.Combat;
using NeoServer.Game.Combat.Attacks.MeleeAttack;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Combat.Attacks.DistanceAttack;

public sealed class DistanceAttackValidation(AttackValidation attackValidation) : IAttackValidation
{
    public Result Validate(AttackInput parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter.Aggressor);
        
        if (parameter.Target is not ICombatActor victim) return Result.NotPossible;
        var aggressor = parameter.Aggressor;

        var result = attackValidation.Validate(aggressor, victim);
        if (result.Failed) return result;
        
        if (aggressor.Location.GetMaxSqmDistance(victim.Location) <= parameter.Attack.Range)
            return Result.Fail(InvalidOperation.TooFar);

        return Result.Success;
    }
}