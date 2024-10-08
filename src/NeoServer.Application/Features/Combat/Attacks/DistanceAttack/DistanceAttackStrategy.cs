using NeoServer.Application.Features.Combat.MonsterDefense;
using NeoServer.Application.Features.Combat.PlayerDefense;
using NeoServer.Game.Combat;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Attacks.DistanceAttack;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Weapons;
using NeoServer.Game.Common.Results;

namespace NeoServer.Application.Features.Combat.Attacks.DistanceAttack;

public sealed class DistanceAttackStrategy(
    DistanceAttackValidation distanceAttackValidation,
    AttackCalculation attackCalculation,
    GameConfiguration gameConfiguration)
    : AttackStrategy
{
    public override string Name => nameof(DistanceAttackStrategy);

    protected override Result Attack(in AttackInput attackInput)
    {
        var aggressor = attackInput.Aggressor;

        if (attackInput.Target is not ICombatActor victim) return Result.NotApplicable;

        var validationResult = distanceAttackValidation.Validate(attackInput);
        if (validationResult.Failed) return validationResult;

        var missAttackResult = CalculateIfMissedAttack(aggressor, victim, attackInput.Parameters);

        aggressor.PreAttack(new PreAttackValues
        {
            Aggressor = aggressor,
            Target = victim,
            MissLocation = missAttackResult.Destination,
            ShootType = attackInput.Parameters.ShootType
        });

        ConsumeAmmo(aggressor);

        var result = true;

        if (!missAttackResult.Missed)
        {
            result = CauseDamage(attackInput, victim);
        }

        aggressor.PostAttack(attackInput);

        if (result) return Result.Success;
        return Result.NotApplicable;
    }

    private void ConsumeAmmo(ICombatActor aggressor)
    {
        if (aggressor is not IPlayer player) return;

        if (player.Inventory.Weapon is IDistanceWeapon && !gameConfiguration.Combat.InfiniteAmmo)
        {
            player.Inventory.Ammo?.Reduce();
        }

        if (player.Inventory.Weapon is IThrowableWeapon throwableDistanceWeapon &&
            throwableDistanceWeapon.ShouldBreak() &&
            !gameConfiguration.Combat.InfiniteThrowingWeapon)
        {
            throwableDistanceWeapon.Reduce();
        }
    }

    private bool CauseDamage(AttackInput attackInput, ICombatActor victim)
    {
        var primaryDamage = attackCalculation.Calculate(attackInput.Parameters.MinDamage,
            attackInput.Parameters.MaxDamage,
            attackInput.Parameters.DamageType);

        var extraAttack = attackInput.Parameters.ExtraAttack;

        //initialize damage list
        Span<CombatDamage> damages = stackalloc CombatDamage[attackInput.Parameters.HasExtraAttack ? 2 : 1];

        damages[0] = primaryDamage;

        if (attackInput.Parameters.HasExtraAttack) AddElementalAttacks(extraAttack, damages);

        if (victim is IPlayer)
        {
            PlayerDefenseHandler.Handle(attackInput.Aggressor, victim as IPlayer, new CombatDamageList(damages));
        }
        
        if (victim is IMonster)
        {
            MonsterDefenseHandler.Handle(attackInput.Aggressor, victim as IMonster, new CombatDamageList(damages));
        }

        return true;
    }

    private static MissAttackResult CalculateIfMissedAttack(ICombatActor aggressor, ICombatActor victim, AttackParameter parameter)
    {
        if (parameter.IsMagicalAttack) return MissAttackResult.NotMissed;
        
        var player = aggressor as IPlayer;

        var missAttackValues = new MissAttackCalculationValues
        (
            origin: aggressor.Location,
            target: victim.Location,
            weapon: player?.Inventory?.Weapon,
            skill: player?.GetSkillLevel(player.SkillInUse) ?? default
        );

        var missAttackResult = MissAttackCalculation.Calculate(missAttackValues);
        return missAttackResult;
    }

    private void AddElementalAttacks(ExtraAttack extraAttack, Span<CombatDamage> damages)
    {
        damages[1] = attackCalculation.Calculate(extraAttack.MinDamage,
            extraAttack.MaxDamage, extraAttack.DamageType);
    }
}