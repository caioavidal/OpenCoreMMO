using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Combat.Attacks
{
    public class DistanceCombatAttack : CombatAttack
    {
        public DistanceCombatAttack(byte range, ShootType shootType)
        {
            Range = range;
            ShootType = shootType;
        }
        public byte Range { get; }
        public ShootType ShootType { get; }

        public static bool CalculateAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option, out CombatDamage damage)
        {
            damage = new CombatDamage();

            if (actor.Location.GetMaxSqmDistance(enemy.Location) > option.Range) return false;

            var damageValue = (ushort)GameRandom.Random.NextInRange(option.MinDamage, option.MaxDamage);

            damage = new CombatDamage(damageValue, option.DamageType);

            return true;
        }

        public static bool MissedAttack(byte hitChance)
        {
            var value = GameRandom.Random.Next(minValue: 1, maxValue: 100);
            return hitChance < value;
        }

        public override bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option, out CombatAttackType combatType)
        {
            combatType = new CombatAttackType(ShootType);

            if (CalculateAttack(actor, enemy, option, out var damage))
            {
                combatType.DamageType = option.DamageType;

                enemy.ReceiveAttack(actor, damage);
                return true;
            }

            return false;
        }
    }
}
