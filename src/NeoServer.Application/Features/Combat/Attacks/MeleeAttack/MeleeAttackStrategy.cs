using Mediator;
using NeoServer.Application.Features.Combat.MonsterDefense;
using NeoServer.Application.Features.Combat.PlayerDefense;
using NeoServer.Game.Combat;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common.Combat;
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

        var physicalDamage = attackCalculation.Calculate(attackInput.Parameters.MinDamage,
            attackInput.Parameters.MaxDamage,
            attackInput.Parameters.DamageType);

        var extraAttack = attackInput.Parameters.ExtraAttack;

        //initialize damage list
        Span<CombatDamage> damages = stackalloc CombatDamage[attackInput.Parameters.HasExtraAttack ? 2 : 1];

        damages[0] = physicalDamage;

        if (attackInput.Parameters.HasExtraAttack) AddElementalAttacks(extraAttack, ref damages);

        aggressor.PreAttack(new PreAttackValues
        {
            Aggressor = aggressor,
            Target = victim
        });

        if (victim is IPlayer)
        {
            PlayerDefenseHandler.Handle(aggressor, victim as IPlayer, new CombatDamageList(damages));
        }
        
        if (victim is IMonster)
        {
            MonsterDefenseHandler.Handle(aggressor, victim as IMonster, new CombatDamageList(damages));
        }
        
        //var result = victim.ReceiveAttackFrom(aggressor, new CombatDamageList(damages));

        aggressor.PostAttack(attackInput);

        return Result.Success;
        // if (result) return Result.Success;
        // return Result.NotApplicable;
    }

    private void AddElementalAttacks(ExtraAttack extraAttack, ref Span<CombatDamage> damages)
    {
        damages[1] = attackCalculation.Calculate(extraAttack.MinDamage,
            extraAttack.MaxDamage, extraAttack.DamageType);
    }
}