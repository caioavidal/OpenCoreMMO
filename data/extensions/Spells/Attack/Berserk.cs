using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack
{
    public class Berserk: AttackSpell
    {
        public override CombatAttack CombatAttack => new ExplosionCombatAttack(3);
        public override MinMax Damage => new(5, 200);
        public override uint Duration => default;
        public override DamageType DamageType => DamageType.Physical;
        public override ConditionType ConditionType => ConditionType.None;
    }
}