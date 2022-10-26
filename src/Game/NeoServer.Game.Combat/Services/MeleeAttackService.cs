using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Combat.Services;

public class MeleeAttackService
{
    private readonly IMapTool _mapTool;

    public MeleeAttackService(IMapTool mapTool)
    {
        _mapTool = mapTool;
    }
    public Result Attack(ICombatActor aggressor, ICombatActor victim)
    {
        var validationResult = AttackValidation.CanAttack(aggressor, victim);
        
        if (validationResult.Failed)
        {
            aggressor.StopAttack();
            return validationResult;
        }
        
        if (!aggressor.CooldownHasExpired(CooldownType.Combat)) return Result.Fail(InvalidOperation.Exhausted);

        aggressor.SetAttackTarget(victim);
        
        if (_mapTool.SightClearChecker?.Invoke(aggressor.Location, victim.Location) == false) return Result.Fail(InvalidOperation.CreatureIsNotReachable);

        if (!aggressor.OnAttack(victim, out var combat)) return Result.NotPossible;

        return Result.Success;
    }
}