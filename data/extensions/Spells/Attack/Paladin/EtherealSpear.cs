using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack.Paladin
{
    public class EtherealSpear: AttackSpell
    {
        public override DamageType DamageType => DamageType.Physical;
        public override CombatAttack CombatAttack =>  new DistanceCombatAttack(Range, ShootType.EtherealSpear);
        public override MinMax CalculateDamage(ICombatActor actor) => new(5, 100);
        public override byte Range => 5;
        public override bool NeedsTarget => true;
    }
}