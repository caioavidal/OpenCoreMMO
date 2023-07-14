using System.Linq;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Effects.Magical;

namespace NeoServer.Game.Combat.Attacks;

public class ExplosionCombatAttack : CombatAttack
{
    public ExplosionCombatAttack(byte radius)
    {
        Radius = radius;
    }

    public byte Radius { get; set; }

    public override bool TryAttack(ICombatActor aggressor, ICombatActor victim, CombatAttackCalculationValue option,
        out CombatAttackParams combatParams)
    {
        combatParams = new CombatAttackParams
        {
            EffectT = option.DamageEffect
        };

        if (CalculateAttack(aggressor, victim, option, out var damage))
        {
            combatParams.DamageType = option.DamageType;
            var location = victim?.Location ?? aggressor.Location;

            var area = ExplosionEffect.Create(location, Radius).ToArray();

            combatParams.SetArea(area);
            aggressor.PropagateAttack(combatParams.Area, damage);
            return true;
        }

        return false;
    }
}