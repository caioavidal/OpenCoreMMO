using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Systems.Combat.Attacks.Player;

public static class PlayerCombatAttack
{
    public static Result Attack(IPlayer player, ICombatActor victim)
    {
        var combatAttack = player.Inventory.IsUsingWeapon ? WeaponCombatAttack.Instance : FistCombatAttack.Instance;
        
        return player.Attack(victim, combatAttack);
    }
}