using NeoServer.Game.Combat.Attacks.MeleeAttack;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Combat.Attacks.Validation;

public class AttackValidation(IMapTool mapTool, IMap map) : IAttackValidation
{
    public Result Validate(ICombatActor aggressor, IThing target)
    {
        Result attackValidationResult = Result.Success;

        if (target is ICombatActor victim)
            attackValidationResult = aggressor.CanAttack(victim);

        if (target is ITile)
            attackValidationResult = aggressor.CanAttack((ITile)target);

        if (target is IItem item)
        {
            var tile = map[item.Location];
            attackValidationResult = aggressor.CanAttack(tile);
        }

        if (attackValidationResult.Failed) return attackValidationResult;

        if (mapTool.SightClearChecker?.Invoke(aggressor.Location, target.Location, true) is false)
        {
            return Result.Fail(InvalidOperation.CannotThrowThere);
        }

        return Result.Success;
    }
}