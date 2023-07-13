using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Systems.Combat.Validations;

namespace NeoServer.Game.Systems.Combat.Combats;

public class AutoAttackSystem
{
    private readonly IMapTool _mapTool;

    public AutoAttackSystem(IMapTool mapTool)
    {
        _mapTool = mapTool;
    }

    public Result RunCombatTurn(ICombatActor aggressor, ICombatActor victim)
    {
        if(aggressor is IMonster) return Result.Success;
        return ExecuteAttack(aggressor, victim);
    }

    private Result ExecuteAttack(ICombatActor aggressor, ICombatActor victim)
    {
        var isAbleToAttack = AggressorIsAbleToAttack(aggressor, victim);
        if (isAbleToAttack.Failed) return isAbleToAttack; //return here if aggressor is not able to attack

        aggressor.SetAttackTarget(victim);

        if (!HasSightClear(aggressor, victim))
            return Result.Fail(InvalidOperation.CreatureIsNotReachable);

        return aggressor.Attack(victim);
    }

    private static Result AggressorIsAbleToAttack(ICombatActor aggressor, ICombatActor victim)
    {
        var result = StopAttackIfCannotAttack(aggressor, victim);
        if (result.Failed)
            return result;

        return !aggressor.Cooldowns.Expired(CooldownType.Combat)
            ? Result.Fail(InvalidOperation.CannotAttackThatFast)
            : Result.Success;
    }

    private bool HasSightClear(ICombatActor aggressor, ICombatActor victim) =>
        _mapTool.SightClearChecker?.Invoke(aggressor.Location, victim.Location, true) ?? false;

    private static Result StopAttackIfCannotAttack(ICombatActor aggressor, ICombatActor victim)
    {
        var canAttackResult = AttackValidation.CanAttack(aggressor, victim);

        if (canAttackResult.Failed)
        {
            aggressor.StopAttack();
        }

        return canAttackResult;
    }
}