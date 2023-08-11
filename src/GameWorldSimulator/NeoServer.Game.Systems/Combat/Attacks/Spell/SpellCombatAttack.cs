using NeoServer.Game.Combat.Validation;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Spells;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Effects.Magical;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Systems.Events;

namespace NeoServer.Game.Systems.Combat.Attacks.Spell;

public class SpellCombatAttack : ISpellCombatAttack
{
    private static IMapTool MapTool { get; set; }
    private static IAreaEffectStore AreaEffectStore { get; set; }

    public static void Setup(IMapTool mapTool, IAreaEffectStore areaEffectStore)
    {
        MapTool = mapTool;
        AreaEffectStore = areaEffectStore;
    }

    public static ISpellCombatAttack Instance { get; } = new SpellCombatAttack();

    public Result CanAttack(IAttackSpell attackSpell, ICombatActor aggressor)
    {
        if (Guard.AnyNull(attackSpell, aggressor)) return Result.NotPossible;

        var target = aggressor.CurrentTarget as ICombatActor;

        if (attackSpell.NeedsTarget && target is null)
        {
            return Result.NotPossible;
        }

        var attackIsValid = AttackIsValid(aggressor, target, attackSpell);

        var combatAttackParams = attackSpell.PrepareAttack(aggressor);

        SetCombatArea(aggressor, combatAttackParams);
        AreaCombatAttackProcessor.Process(aggressor, combatAttackParams);

        if (!combatAttackParams.Area.HasAnyLocationAffected)
        {
            if (aggressor is IPlayer player) OperationFailService.Send(player, InvalidOperation.CannotThrowThere);
            return new Result(InvalidOperation.CannotThrowThere);
        }

        return attackIsValid;
    }

    public Result CauseDamage(IAttackSpell attackSpell, ICombatActor aggressor, ICombatActor victim)
    {
        if (Guard.AnyNull(attackSpell, aggressor)) return Result.NotPossible;

        AreaCombatAttackProcessor.LastCombatAttackParamProcessed.Remove(aggressor.CreatureId,
            out var combatAttackParams);

        combatAttackParams ??= attackSpell.PrepareAttack(aggressor);

        if (combatAttackParams is null) return Result.NotApplicable;

        bool attackResult;

        if (combatAttackParams.IsAreaAttack || aggressor.CurrentTarget is not ICombatActor)
        {
            attackResult = SpreadAttackToArea(aggressor, combatAttackParams);
            return attackResult ? Result.Success : Result.NotPossible;
        }

        attackResult = ExecuteTargetAttack(aggressor, victim, combatAttackParams);

        return attackResult ? Result.Success : Result.NotPossible;
    }

    private static bool ExecuteTargetAttack(ICombatActor aggressor, ICombatActor victim,
        CombatAttackParams combatAttackParams)
    {
        var attackResult = true;
        CombatEvent.InvokeOnAttackingEvent(aggressor, victim, new[] { combatAttackParams });

        foreach (var damage in combatAttackParams.Damages)
        {
            attackResult &= victim.ReceiveAttack(aggressor, damage);
        }

        return attackResult;
    }

    private static void SetCombatArea(ICombatActor aggressor, CombatAttackParams combatAttackParams)
    {
        if (combatAttackParams.Area?.Any() ?? false) return;

        var nextLocation = aggressor.Location.GetNextLocation(aggressor.Direction);

        if (combatAttackParams.IsAreaAttack is false) //set area to one sqm ahead if there is no area set
        {
            combatAttackParams.SetArea(new Coordinate[] { new(nextLocation) });
            return;
        }

        var hasAreaName = !string.IsNullOrWhiteSpace(combatAttackParams.AreaName);

        if (hasAreaName)
        {
            var location = AreaEffectStore.IsWaveEffect(combatAttackParams.AreaName)
                ? nextLocation
                : aggressor.Location; //wave effect starts one sqm ahead

            var template = AreaEffectStore.Get(combatAttackParams.AreaName, aggressor.Direction);
            combatAttackParams.SetArea(AreaEffect.Create(location, template));
        }
    }

    private static bool SpreadAttackToArea(ICombatActor aggressor, CombatAttackParams combatAttackParams)
    {
        SetCombatArea(aggressor, combatAttackParams);
        return AreaCombatAttack.PropagateAttack(aggressor, combatAttackParams);
    }

    private static Result AttackIsValid(ICombatActor aggressor, ICombatActor victim, IAttackSpell spell)
    {
        var result = AttackValidation.CanAttack(aggressor);

        if (result.Failed || (victim?.IsInvisible ?? false))
        {
            aggressor.StopAttack();
            return result;
        }

        if (spell.NeedsTarget && MapTool.SightClearChecker?.Invoke(aggressor.Location, victim.Location, true) == false)
        {
            OperationFailService.Send(aggressor.CreatureId, "You cannot throw there.");
            return Result.NotPossible;
        }

        return Result.Success;
    }
}