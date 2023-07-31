using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Creatures;
using NeoServer.Game.Systems.Events;

namespace NeoServer.Game.Systems.Combat.Attacks.Player;

internal class WeaponCombatAttack : ICombatAttack2
{
    public static ICombatAttack2 Instance { get; } = new WeaponCombatAttack();
    public CombatType CombatType => CombatType.Weapon;
    public Result CauseDamage(IThing aggressor, IThing enemy)
    {
        if (aggressor is not IPlayer player) return Result.NotApplicable;
        if (player.Inventory.Weapon is not {} weapon) return Result.NotApplicable;

        if (enemy is not ICombatActor victim) return Result.NotApplicable;

        if (!weapon.CanAttack(player, victim)) return Result.NotPossible;

        var combatAttack = weapon.GetAttackParameters(player, victim);

        weapon.PreAttack(player, victim);

        if (!CombatIsOk(combatAttack, out var result)) return result;

        CombatEvent.InvokeOnAttackingEvent(player, victim, new[] { combatAttack });
        
        foreach (var damage in combatAttack.Damages)
        {
            victim.ReceiveAttack(player, damage);
            player.PropagateAttack(combatAttack.Area, damage);
        }

        weapon.PostAttack(player, victim);
        
        return Result.Success;
    }

    private static bool CombatIsOk(CombatAttackParams combatAttack, out Result result)
    {
        result = Result.Success;
        
        if (combatAttack is null)
        {
            result = Result.Fail(InvalidOperation.NotPossible);
            return false;
        }

        if (combatAttack.Missed)
        {
            result = Result.Success;
            return false;
        }

        if (combatAttack.Damages is null)
        {
            result = Result.NotPossible;
            return false;
        }

        return true;
    }
}