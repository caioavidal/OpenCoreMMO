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

    public override bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackCalculationValue option,
        out CombatAttackParams combatParams)
    {
        combatParams = new CombatAttackParams(option.DamageType)
        {
            EffectT = option.DamageEffect
        };

        if (CalculateAttack(actor, enemy, option, out var damage))
        {
            combatParams.SetArea(SpreadEffect.Create(actor.Location, actor.Direction, Length, Spread));
            actor.PropagateAttack(combatParams.Area, damage);
            return true;
        }

        return false;
    }
}