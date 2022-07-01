using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack
{
    public class EnergyStrike: AttackSpell
    {
        public override DamageType DamageType => DamageType.Energy;
        public override CombatAttack CombatAttack =>  new DistanceCombatAttack(Range, ShootType.Energy);
        public override MinMax CalculateDamage(ICombatActor actor) => new(5, 100);
        public override byte Range => 5;
    }
}