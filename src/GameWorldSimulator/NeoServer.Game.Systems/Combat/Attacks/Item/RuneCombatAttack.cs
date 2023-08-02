using NeoServer.Game.Combat.Validation;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Systems.Events;

namespace NeoServer.Game.Systems.Combat.Attacks.Item;

internal class RuneCombatAttack : IItemCombatAttack
{
    private static IMapTool MapTool { get; set; }

    public static void Setup(IMapTool mapTool)
    {
        MapTool = mapTool;
    }

    public static IItemCombatAttack Instance { get; } = new RuneCombatAttack();

    public CombatType CombatType => CombatType.Rune;

    public Result CauseDamage(IItem item, IThing aggressor, IThing enemy)
    {
        if (aggressor is not IPlayer player) return Result.NotApplicable;

        if (enemy is IDynamicTile tile)
        {
            if(tile.TopCreatureOnStack is null) return new Result(InvalidOperation.NeedsTarget);
            enemy = tile.TopCreatureOnStack;
        }
        
        if (enemy is not ICombatActor victim) return new Result(InvalidOperation.NeedsTarget);

        if (item is not IAttackRune { NeedTarget: true } rune) return Result.NotApplicable;

        var attackIsValid = AttackIsValid(player, victim, rune);

        if (attackIsValid.Failed) return attackIsValid;

        var combatAttackParams = rune.PrepareAttack(player);

        CombatEvent.InvokeOnAttackingEvent(player, victim, new[] { combatAttackParams });

        var attackResult = true;

        foreach (var damage in combatAttackParams.Damages)
        {
            attackResult &= victim.ReceiveAttack(player, damage);
        }

        return attackResult ? Result.Success : Result.NotPossible;
    }

    private static Result AttackIsValid(IPlayer player, ICombatActor victim, IAttackRune rune)
    {
        var result = AttackValidation.CanAttack(player, victim);

        if (result.Failed || victim.IsInvisible)
        {
            player.StopAttack();
            return result;
        }

        if (rune.NeedTarget && MapTool.SightClearChecker?.Invoke(player.Location, victim.Location, true) == false)
        {
            OperationFailService.Send(player.CreatureId, "You cannot throw there.");
            return Result.NotPossible;
        }

        return Result.Success;
    }
}