using NeoServer.Game.Combat.Conditions;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Effects.Parsers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Parsers;

namespace NeoServer.Game.Combat.Attacks.DamageConditionAttack;

public class DamageConditionAttack
{
    public DamageConditionAttack(DamageType damageType, MinMax damage, byte damageCount,
        int interval)
    {
        DamageType = damageType;
        MinDamage = (ushort)damage.Min;
        MaxDamage = (ushort)damage.Max;
        DamageCount = damageCount;
        Interval = interval;
    }

    private DamageType DamageType { get; }
    private ushort MinDamage { get; }
    private ushort MaxDamage { get; }
    private byte DamageCount { get; }
    private int Interval { get; }

    public void CauseDamage(IThing aggressor, ICreature toCreature)
    {
        if (toCreature is not ICombatActor actor) return;
        if (MaxDamage == 0) return;

        var conditionType = ConditionTypeParser.Parse(DamageType);

        actor.ReceiveAttack(aggressor,
            new CombatDamage(MaxDamage, DamageType) { Effect = DamageEffectParser.Parse(DamageType) });

        if (actor.HasCondition(conditionType, out var condition) && condition is DamageCondition damageCondition)
        {
            if (DamageCount == 0) damageCondition.Start(toCreature, MinDamage, MaxDamage);
            else damageCondition.Restart(DamageCount);

            return;
        }

        actor.AddCondition(DamageCount == 0
            ? new DamageCondition(aggressor, conditionType, Interval, MinDamage, MaxDamage)
            : new DamageCondition(aggressor, conditionType, Interval, DamageCount, MinDamage));
    }
}