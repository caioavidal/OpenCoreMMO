using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Systems.Events;

namespace NeoServer.Game.Systems.Combat.Attacks.Player;

internal class FistCombatAttack: ICombatAttack2
{
    public CombatType CombatType => CombatType.Fist;
    public static ICombatAttack2 Instance { get; } = new FistCombatAttack();

    public Result CauseDamage(IThing aggressor, IThing enemy)
    {
        if (aggressor is not IPlayer player) return Result.NotApplicable;
        
        if (enemy is not ICombatActor victim) return Result.NotApplicable;

        if (!GetAttackParams(victim, player, out var combatAttackParams)) return Result.NotPossible;

        CombatEvent.InvokeOnAttackingEvent(player, victim, new[] { combatAttackParams });
        
        foreach (var damage in combatAttackParams.Damages)
        {
            victim.ReceiveAttack(player, damage);
        }

        return Result.Success;
    }
    private static bool GetAttackParams(ICombatActor victim, IPlayer player, out CombatAttackParams combatAttackParams)
    {
        combatAttackParams = new CombatAttackParams(DamageType.Melee);

        var maxDamage = player.CalculateAttackPower(0.085f, 7);
        var combat = new CombatAttackCalculationValue(player.MinimumAttackPower,
            maxDamage, DamageType.Melee);

        if(!MeleeCombatAttack.CalculateAttack(player, victim, combat, out var damage)) return false;

        combatAttackParams.Damages = new[] { damage };

        return true;
    }
}