using NeoServer.Game.Combat.Validation;
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

    public Result CauseDamage(IAttackSpell attackSpell, ICombatActor aggressor, ICombatActor victim)
    {
        if (Guard.AnyNull(attackSpell, aggressor)) return Result.NotPossible;

        var target = aggressor.CurrentTarget as ICombatActor;

        if (attackSpell.NeedsTarget && target is null)
        {
            return Result.NotPossible;
        }

        var attackIsValid = AttackIsValid(aggressor, victim, attackSpell);

        if (attackIsValid.Failed) return attackIsValid;

        var combatAttackParams = attackSpell.PrepareAttack(aggressor);

        if (combatAttackParams is null) return Result.NotApplicable;

        bool attackResult;

        if (combatAttackParams.IsAreaAttack || target is null)
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

    private static bool SpreadAttackToArea(ICombatActor aggressor, CombatAttackParams combatAttackParams)
    {
        var nextLocation = aggressor.Location.GetNextLocation(aggressor.Direction);

        if (combatAttackParams.IsAreaAttack is false) //set area to one sqm ahead if there is no area set
        {
            combatAttackParams.SetArea(new Coordinate[] { new(nextLocation) });

            return AreaCombatAttack.PropagateAttack(aggressor, combatAttackParams);
        }

        var hasAreaName = !string.IsNullOrWhiteSpace(combatAttackParams.AreaName);

        if (hasAreaName)
        {
            var location = AreaEffectStore.IsWaveEffect(combatAttackParams.AreaName) ? nextLocation : aggressor.Location; //wave effect starts one sqm ahead
            
            var template = AreaEffectStore.Get(combatAttackParams.AreaName, aggressor.Direction);
            combatAttackParams.SetArea(AreaEffect.Create(location, template));
        }

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