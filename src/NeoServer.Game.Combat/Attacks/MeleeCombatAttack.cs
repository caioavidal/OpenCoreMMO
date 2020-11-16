using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Item;
using NeoServer.Server.Helpers;
using System;

namespace NeoServer.Game.Combat.Attacks
{
    public class MeleeCombatAttack : CombatAttack
    {
        public static bool CalculateAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue combat, out CombatDamage damage)
        {
            damage = new CombatDamage();
            if (!actor.Location.IsNextTo(enemy.Location)) return false;

            var damageValue = (ushort)ServerRandom.Random.NextInRange(combat.MinDamage, combat.MaxDamage);

            damage = new CombatDamage(damageValue, combat.DamageType);

            return true;
        }

        public static ushort CalculateMaxDamage(int skill, int attack) => (ushort)Math.Ceiling((skill * (attack * 0.05)) + (attack * 0.5));
        
    }
}
