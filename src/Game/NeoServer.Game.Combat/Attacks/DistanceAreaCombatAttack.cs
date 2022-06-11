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

    public override bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option,
        out CombatAttackResult combatResult)
    {
        combatResult = new CombatAttackResult(ShootType)
        {
            EffectT = option.DamageEffect
        };

        if (CalculateAttack(actor, enemy, option, out var damage))
        {
            combatResult.DamageType = option.DamageType;
            combatResult.SetArea(ExplosionEffect.Create(enemy.Location, Radius).ToArray());
            actor.PropagateAttack(combatResult.Area, damage);
            return true;
        }

        return false;
    }
}