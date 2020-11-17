using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Combat.Attacks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Server.Helpers;
using System;

namespace NeoServer.Game.Combat.Attacks
{

    public abstract class CombatAttack : ICombatAttack
    {
        public virtual bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option, out CombatAttackType combatType)
        {
            combatType = new CombatAttackType();
            return false;
        }
        public static bool CalculateAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option, out CombatDamage damage)
        {
            damage = new CombatDamage();

            var damageValue = (ushort)ServerRandom.Random.NextInRange(option.MinDamage, option.MaxDamage);
            damage = new CombatDamage(damageValue, option.DamageType);

            return true;
        }
    }
}
