using NeoServer.Game.Combat;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Attacks.DistanceAttack;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Weapons;
using NeoServer.Game.Common.Results;

namespace NeoServer.Application.Features.Combat.Attacks.DistanceAttack;

public sealed class DistanceWeaponAttackStrategy(
    DistanceAttackValidation distanceAttackValidation,
    AttackCalculation attackCalculation,
    GameConfiguration gameConfiguration)
    : AttackStrategy
{
    public override string Name => nameof(DistanceWeaponAttackStrategy);

    protected override Result Attack(in AttackInput attackInput)
    {
        var aggressor = attackInput.Aggressor;

        if (attackInput.Target is not ICombatActor victim) return Result.NotApplicable;

        var validationResult = distanceAttackValidation.Validate(attackInput);
        if (validationResult.Failed) return validationResult;

        var missAttackResult = CalculateIfMissedAttack(aggressor, victim);

        aggressor.PreAttack(new PreAttackValues
        {
            Aggressor = aggressor,
            Target = victim,
            MissLocation = missAttackResult.Destination,
            ShootType = attackInput.Attack.ShootType
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
        var physicalDamage = attackCalculation.Calculate(attackInput.Attack.MinDamage,
            attackInput.Attack.MaxDamage,
            attackInput.Attack.DamageType);

        var extraAttack = attackInput.Attack.ExtraAttack;

        //initialize damage list
        Span<CombatDamage> damages = stackalloc CombatDamage[attackInput.Attack.HasExtraAttack ? 2 : 1];

        damages[0] = physicalDamage;

        if (attackInput.Attack.HasExtraAttack) AddElementalAttacks(extraAttack, damages);

        var result = victim.ReceiveAttackFrom(attackInput.Aggressor, new CombatDamageList(damages));
        return result;
    }

    private static MissAttackResult CalculateIfMissedAttack(ICombatActor aggressor, ICombatActor victim)
    {
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