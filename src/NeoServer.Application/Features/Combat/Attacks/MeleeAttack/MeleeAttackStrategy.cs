using NeoServer.Game.Combat;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Attacks.MeleeAttack;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Results;

namespace NeoServer.Application.Features.Combat.Attacks.MeleeAttack;

public sealed class MeleeAttackStrategy(
    MeleeAttackValidation meleeAttackValidation,
    AttackCalculation attackCalculation)
    : AttackStrategy
{
    public override string Name { get; } = nameof(MeleeAttackStrategy);

    protected override Result Attack(in AttackInput attackInput)
    {
        var aggressor = attackInput.Aggressor;

        if (attackInput.Target is not ICombatActor victim) return Result.NotApplicable;

        var validationResult = meleeAttackValidation.Validate(aggressor, victim);
        if (validationResult.Failed) return validationResult;

        var physicalDamage = attackCalculation.Calculate(attackInput.Attack.MinDamage,
            attackInput.Attack.MaxDamage,
            attackInput.Attack.DamageType);

        var extraAttack = attackInput.Attack.ExtraAttack;

        //initialize damage list
        Span<CombatDamage> damages = stackalloc CombatDamage[attackInput.Attack.HasExtraAttack ? 2 : 1];

        damages[0] = physicalDamage;

        if (attackInput.Attack.HasExtraAttack) AddElementalAttacks(extraAttack, ref damages);

        aggressor.PreAttack(new PreAttackValues
        {
            Aggressor = aggressor,
            Target = victim
        });

        var result = victim.ReceiveAttackFrom(aggressor, new CombatDamageList(damages));

        aggressor.PostAttack(attackInput);

        if (result) return Result.Success;
        return Result.NotApplicable;
    }

    private void AddElementalAttacks(ExtraAttack extraAttack, ref Span<CombatDamage> damages)
    {
        damages[1] = attackCalculation.Calculate(extraAttack.MinDamage,
            extraAttack.MaxDamage, extraAttack.DamageType);
    }
}