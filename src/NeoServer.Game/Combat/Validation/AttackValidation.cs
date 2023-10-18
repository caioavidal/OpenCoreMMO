using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Combat.Validation;

public static class AttackValidation
{
    public static Result CanAttack(ICombatActor aggressor, ICombatActor victim)
    {
        if (Guard.AnyNull(aggressor, victim) || aggressor.Equals(victim)) return Result.NotPossible;

        if (victim.IsDead || aggressor.IsDead) return Result.Fail(InvalidOperation.CreatureIsDead);

        if (!aggressor.CanSee(victim.Location)) return Result.Fail(InvalidOperation.CreatureIsNotReachable);

        if (!aggressor.Location.SameFloorAs(victim.Location))
            return Result.Fail(InvalidOperation.CreatureIsNotReachable);

        if (aggressor.Tile?.ProtectionZone ?? false)
            return Result.Fail(InvalidOperation.CannotAttackWhileInProtectionZone);

        if (victim.Tile?.ProtectionZone ?? false)
            return Result.Fail(InvalidOperation.CannotAttackPersonInProtectionZone);

        return Result.Success;
    }
}