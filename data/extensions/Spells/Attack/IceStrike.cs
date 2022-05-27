using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack
{
    public class IceStrike: AttackSpell
    {
        public override DamageType DamageType => DamageType.Ice;
        public override CombatAttack CombatAttack =>  new DistanceCombatAttack(Range, ShootType.Ice);
        public override MinMax Damage => new(5, 100);
        public override byte Range => 5;
    }
}