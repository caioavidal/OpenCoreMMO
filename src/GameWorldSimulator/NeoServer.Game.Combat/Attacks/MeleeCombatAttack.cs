using System;
using NeoServer.Game.Combat.Conditions;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Game.Combat.Attacks;

public class MeleeCombatAttack : CombatAttack
{
    public MeleeCombatAttack(ushort min, ushort max, ConditionType conditionType, ushort conditionInterval)
    {
        Min = min;
        Max = max;
        ConditionType = conditionType;
        ConditionInterval = conditionInterval;
    }

    public MeleeCombatAttack()
    {
    }

    public ushort Min { get; set; }
    public ushort Max { get; set; }
    public ConditionType ConditionType { get; set; }
    public ushort ConditionInterval { get; set; }

    public static bool CalculateAttack(ICombatActor actor, ICombatActor enemy, CombatAttackCalculationValue combat,
        out CombatDamage damage)
    {
        damage = new CombatDamage();
        if (!actor.Location.IsNextTo(enemy.Location)) return false;

        var damageValue = (ushort)GameRandom.Random.NextInRange(combat.MinDamage, combat.MaxDamage);

        damage = new CombatDamage(damageValue, combat.DamageType);

        return true;
    }

    public static ushort CalculateMaxDamage(int skill, int attack)
    {
        return (ushort)Math.Ceiling(skill * (attack * 0.05) + attack * 0.5);
    }

    public override bool TryAttack(ICombatActor aggressor, ICombatActor victim, CombatAttackCalculationValue option,
        out CombatAttackParams combatParams)
    {
        combatParams = new CombatAttackParams(option.DamageType);

        if (CalculateAttack(aggressor, victim, option, out var damage))
        {
            var wasDamaged = victim.ReceiveAttack(aggressor, damage);

            if (!wasDamaged) return true;

            if (ConditionType != ConditionType.None)
            {
                if (!victim.HasCondition(ConditionType, out var condition))
                    victim.AddCondition(new DamageCondition(aggressor,ConditionType, ConditionInterval, Min, Max));
                else if (condition is DamageCondition damageCondition) damageCondition.Start(victim, Min, Max);
                else condition.Start(victim);
            }

            return true;
        }

        return false;
    }
}