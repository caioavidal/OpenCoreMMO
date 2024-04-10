using NeoServer.Game.Combat.Attacks.MeleeAttack;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Results;

namespace NeoServer.Application.Features.Combat;

public class AttackValidation(IMapTool mapTool) : IAttackValidation
{
    public Result Validate(ICombatActor aggressor, ICombatActor victim)
    {
        var attackValidationResult = aggressor.CanAttack(victim);

        if (attackValidationResult.Failed) return attackValidationResult;
        
        if (mapTool.SightClearChecker?.Invoke(aggressor.Location, victim.Tile.Location, true) is false)
        {
            return Result.Fail(InvalidOperation.CannotThrowThere);
        }

        return Result.Success;
    }
}