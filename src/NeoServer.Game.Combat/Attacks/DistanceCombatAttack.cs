using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Server.Helpers;
using System;

namespace NeoServer.Game.Combat.Attacks
{
    public class DistanceCombatAttack : CombatAttack
    {
        public static bool CalculateAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option, out CombatDamage damage)
        {
            damage = new CombatDamage();

            if (actor.Location.GetSqmDistanceX(enemy.Location) > option.Range || actor.Location.GetSqmDistanceY(enemy.Location) > option.Range) return false;

            var damageValue = (ushort)ServerRandom.Random.NextInRange(option.MinDamage, option.MaxDamage);


            damage = new CombatDamage(damageValue, option.DamageType);

            return true;
        }

        public static bool MissedAttack(byte hitChance)
        {
            var value = ServerRandom.Random.Next(minValue: 1, maxValue: 100);
            return hitChance < value;
        }
    }
}
