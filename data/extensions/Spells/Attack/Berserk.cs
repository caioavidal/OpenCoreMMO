using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack;

public class Berserk : AttackSpell
{
    public override CombatAttack CombatAttack => new ExplosionCombatAttack(3);
    public override uint Duration => default;
    public override DamageType DamageType => DamageType.MagicalPhysical;
    public override ConditionType ConditionType => ConditionType.None;

    public override MinMax CalculateDamage(ICombatActor actor)
    {
        return new MinMax(5, 100);
    }
}