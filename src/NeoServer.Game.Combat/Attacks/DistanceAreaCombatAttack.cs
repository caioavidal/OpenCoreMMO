
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Effects.Explosion;
using System.Linq;

namespace NeoServer.Game.Creatures.Combat.Attacks
{
    public class DistanceAreaCombatAttack : DistanceCombatAttack
    {
        public DistanceAreaCombatAttack(byte range, byte radius, ShootType shootType) : base(range, shootType)
        {
            Radius = radius;
        }

        public override bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option, out CombatAttackType combatType)
        {
            combatType = new CombatAttackType(ShootType);

            if (CalculateAttack(actor, enemy, option, out var damage))
            {
                combatType.DamageType = option.DamageType;
                combatType.Area = ExplosionEffect.Create(enemy.Location, Radius).ToArray();
                actor.PropagateAttack(combatType.Area, damage);
                return true;
            }

            return false;
        }

        public byte Radius { get; set; }
    }
}
