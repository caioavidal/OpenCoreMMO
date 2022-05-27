using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack
{
    public class FlameStrike: AttackSpell
    {
        public override DamageType DamageType => DamageType.Fire;
        public override CombatAttack CombatAttack =>  new DistanceCombatAttack(Range, ShootType.Fire);
        public override MinMax Damage => new(5, 100);
        public override byte Range => 5;
    }
}