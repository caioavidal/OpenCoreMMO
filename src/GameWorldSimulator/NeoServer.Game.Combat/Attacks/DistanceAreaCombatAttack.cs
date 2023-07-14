using System.Linq;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Effects.Magical;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Combat.Attacks;

public class DistanceAreaCombatAttack : DistanceCombatAttack
{
    public DistanceAreaCombatAttack(byte range, byte radius, ShootType shootType) : base(range, shootType)
    {
        Radius = radius;
    }

    public byte Radius { get; set; }

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
            combatParams.SetArea(ExplosionEffect.Create(victim.Location, Radius).ToArray());
            aggressor.PropagateAttack(combatParams.Area, damage);
            return true;
        }

        return false;
    }
}