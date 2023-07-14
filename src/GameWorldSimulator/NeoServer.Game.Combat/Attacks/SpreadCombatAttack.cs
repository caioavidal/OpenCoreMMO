using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Effects.Magical;

namespace NeoServer.Game.Combat.Attacks;

public class SpreadCombatAttack : CombatAttack
{
    public SpreadCombatAttack(byte length, byte spread)
    {
        Length = length;
        Spread = spread;
    }

    public byte Spread { get; }
    public byte Length { get; }

    public override bool TryAttack(ICombatActor aggressor, ICombatActor victim, CombatAttackCalculationValue option,
        out CombatAttackParams combatParams)
    {
        combatParams = new CombatAttackParams(option.DamageType)
        {
            EffectT = option.DamageEffect
        };

        if (CalculateAttack(aggressor, victim, option, out var damage))
        {
            combatParams.SetArea(SpreadEffect.Create(aggressor.Location, aggressor.Direction, Length, Spread));
            aggressor.PropagateAttack(combatParams.Area, damage);
            return true;
        }

        return false;
    }
}