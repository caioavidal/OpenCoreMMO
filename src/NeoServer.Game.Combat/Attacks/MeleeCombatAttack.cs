using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Item;
using NeoServer.Server.Helpers;

namespace NeoServer.Game.Combat.Attacks
{
    public class MeleeCombatAttack : CombatAttack<MeleeCombatAttack>
    {
        public bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue combat, out CombatDamage damage)
        {
            damage = new CombatDamage();
            if (!actor.Location.IsNextTo(enemy.Location)) return false;

            var damageValue = (ushort)ServerRandom.Random.NextInRange(combat.MinDamage, combat.MaxDamage);

            damage = new CombatDamage(damageValue, combat.DamageType);

            return true;
        }
    }
}
