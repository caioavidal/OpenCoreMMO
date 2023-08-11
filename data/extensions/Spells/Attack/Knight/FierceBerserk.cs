using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Items.Items.Weapons;
using NeoServer.Game.Systems.Combat.Attacks.Spell;

namespace NeoServer.Extensions.Spells.Attack.Knight;

public class FierceBerserk: AttackSpell
{
    public override DamageType DamageType => DamageType.MagicalPhysical;
    public override string AreaName => "AREA_SQUARE3X3";
    public override ISpellCombatAttack CombatAttack => SpellCombatAttack.Instance;

    public override Result CanCast(ICombatActor actor)
    {
        if (actor is not IPlayer player) return Result.NotApplicable;

        var canCastResult = base.CanCast(actor);

        if (canCastResult.Failed) return canCastResult;

        var weapon = player.Inventory.Weapon;

        if (weapon is MeleeWeapon) return Result.Success;

        OperationFailService.Send(player, "You need to equip a weapon to use this spell.");
        return Result.NotPossible;
    }

    public override MinMax GetFormula(ICombatActor actor)
    {
        if (actor is not IPlayer player) return new MinMax(0, 0);
        
        var attack = player.Inventory.TotalAttack;
        var skill = player.Skills[player.SkillInUse].Level;
        
        var min = player.MinimumAttackPower + skill * attack * 0.06 + 13;
        var max = player.MinimumAttackPower + skill * attack * 0.11 + 27;

        return new MinMax((int)min, (int)max);
    }
}