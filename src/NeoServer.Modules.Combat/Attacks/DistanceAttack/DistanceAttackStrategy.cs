using NeoServer.Game.Combat;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Results;
using NeoServer.Modules.Combat.Attacks.InAreaAttack;
using NeoServer.Modules.Combat.MonsterDefense;
using NeoServer.Modules.Combat.PlayerDefense;

namespace NeoServer.Modules.Combat.Attacks.DistanceAttack;

public sealed class DistanceAttackStrategy(
    DistanceAttackValidation distanceAttackValidation,
    AttackCalculation attackCalculation,
    AreaAttackProcessor areaAttackProcessor)
    : AttackStrategy
{
    public override string Name => nameof(DistanceAttackStrategy);

    protected override Result Attack(in AttackInput attackInput)
    {
        var aggressor = attackInput.Aggressor;
        var target = attackInput.Target;

        if (attackInput.Parameters.NeedTarget && target is not ICombatActor) 
            return Result.NotApplicable;

        var validationResult = distanceAttackValidation.Validate(attackInput);
        if (validationResult.Failed) return validationResult;

        var missAttackResult = CalculateIfMissedAttack(aggressor, target, attackInput.Parameters);

        aggressor.PreAttack(new PreAttackValues
        {
            Aggressor = aggressor,
            Target = target,
            MissLocation = missAttackResult.Destination,
            ShootType = attackInput.Parameters.ShootType
        });

        var result = true;

        if (!missAttackResult.Missed)
        {
            result = ApplyDamage(attackInput, target);
        }

        aggressor.PostAttack(attackInput);
        return result ? Result.Success : Result.NotApplicable;
    }

    private bool ApplyDamage(AttackInput attackInput, IThing victim)
    {
        var primaryDamage = attackCalculation.Calculate(attackInput.Parameters.MinDamage,
            attackInput.Parameters.MaxDamage,
            attackInput.Parameters.DamageType);

        var extraAttack = attackInput.Parameters.ExtraAttack;

        //initialize damage list
        Span<CombatDamage> damages = stackalloc CombatDamage[attackInput.Parameters.HasExtraAttack ? 2 : 1];

        damages[0] = primaryDamage;

        if (attackInput.Parameters.HasExtraAttack) AddElementalAttacks(extraAttack, damages);

        if (attackInput.Parameters.IsAttackInArea)
        {
            areaAttackProcessor.Propagate(attackInput, new CombatDamageList(damages));
            return true;
        }

        switch (victim)
        {
            case IPlayer player:
                PlayerDefenseHandler.Handle(attackInput.Aggressor, player, new CombatDamageList(damages));
                break;
            case IMonster monster:
                MonsterDefenseHandler.Handle(attackInput.Aggressor, monster, new CombatDamageList(damages));
                break;
        }

        return true;
    }

    private static MissAttackResult CalculateIfMissedAttack(ICombatActor aggressor, IThing victim,
        AttackParameter parameter)
    {
        if (parameter.IsMagicalAttack) return MissAttackResult.NotMissed;

        var player = aggressor as IPlayer;

        var missAttackValues = new MissAttackCalculationValues
        (
            aggressor.Location,
            victim.Location,
            player?.Inventory?.Weapon,
            player?.GetSkillLevel(player.SkillInUse) ?? default
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