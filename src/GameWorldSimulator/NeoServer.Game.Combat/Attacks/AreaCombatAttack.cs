using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Effects.Magical;

namespace NeoServer.Game.Combat.Attacks;

public class AreaCombatAttack : CombatAttack
{
    private readonly byte[,] _area;

    public AreaCombatAttack(byte[,] area)
    {
        _area = area;
    }

    public override bool TryAttack(ICombatActor aggressor, ICombatActor victim, CombatAttackCalculationValue option,
        out CombatAttackParams combatParams)
    {
        combatParams = new CombatAttackParams();

        if (CalculateAttack(aggressor, victim, option, out var damage))
        {
            combatParams.DamageType = option.DamageType;
            combatParams.EffectT = option.DamageEffect;

            var area = AreaEffect.Create(aggressor.Location, _area);
            combatParams.SetArea(area);
            aggressor.PropagateAttack(combatParams.Area, damage);
            return true;
        }

        return false;
    }
}