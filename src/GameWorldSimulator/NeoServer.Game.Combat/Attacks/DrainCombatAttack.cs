using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Combat.Attacks;

public class DrainCombatAttack : DistanceAreaCombatAttack
{
    public DrainCombatAttack(byte range, byte radius, ShootType shootType) : base(range, radius, shootType)
    {
    }

    public override bool TryAttack(ICombatActor aggressor, ICombatActor victim, CombatAttackCalculationValue option,
        out CombatAttackParams combatParams)
    {
        combatParams = new CombatAttackParams(ShootType)
        {
            EffectT = option.DamageEffect
        };

        if (CalculateAttack(aggressor, victim, option, out var damage))
        {
            combatParams.DamageType = option.DamageType;

            victim.ReceiveAttack(aggressor, damage);
            return true;
        }

        return false;
    }
}