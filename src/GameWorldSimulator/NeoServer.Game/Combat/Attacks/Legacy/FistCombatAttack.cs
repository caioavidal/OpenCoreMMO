using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Combat.Attacks.Legacy;

public static class FistCombatAttack
{
    public static bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackResult combatResult)
    {
        combatResult = new CombatAttackResult(DamageType.Melee);

        if (actor is not IPlayer player) return false;

        var maxDamage = player.MaximumAttackPower;
        var combat = new CombatAttackValue(actor.MinimumAttackPower,
            maxDamage, DamageType.Melee);

        if (!MeleeCombatAttack.CalculateAttack(actor, enemy, combat, out var damage)) return false;

        enemy.ReceiveAttackFrom(actor, damage);

        return true;
    }
}