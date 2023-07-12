using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Combat.Attacks;

public class DrainCombatAttack : DistanceAreaCombatAttack
{
    public DrainCombatAttack(byte range, byte radius, ShootType shootType) : base(range, radius, shootType)
    {
    }

    public override bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackCalculationValue option,
        out CombatAttackParams combatParams)
    {
        combatParams = new CombatAttackParams(ShootType)
        {
            EffectT = option.DamageEffect
        };

        if (CalculateAttack(actor, enemy, option, out var damage))
        {
            combatParams.DamageType = option.DamageType;

            enemy.ReceiveAttack(actor, damage);
            return true;
        }

        return false;
    }
}